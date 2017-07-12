using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using HtmlAgilityPack;
using log4net.Config;
using Microsoft.Azure;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using ProtoBuf;

namespace ConsoleAppCrawler
{

    class Program
    {
        //https://stackoverflow.com/questions/42581658/crawl-sitemap-with-abot
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            var dummyList = new List<SearchModel>(2000);
            var list = new ObservableCollection<SearchModel>(dummyList);
            var lockObject = new object();


            var crawler = new Crawler(list, lockObject);
            var t = Task.Run(() => crawler.DoStuffAsync());

            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the queue client.
            var queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            var queue = queueClient.GetQueueReference("bing-test");


            var m_SearchServiceClient = new SearchServiceClient("cloudents", new SearchCredentials("5B0433BFBBE625C9D60F7330CFF103F0"));
            var indexClient = m_SearchServiceClient.Indexes.GetClient("bing-test");

            var client = new DocumentClient(new Uri("https://zboxnew.documents.azure.com:443/"), "y2v1XQ6WIg81Soasz5YBA7R8fAp52XhJJufNmHy1t7y3YQzpBqbgRnlRPlatGhyGegKdsLq0qFChzOkyQVYdLQ==");
            var tableUri = UriFactory.CreateDocumentCollectionUri("Zbox", "Crawl-Url");

            var t2 = Task.Run(async () =>
            {

                var needContinue = true;
                while (!t.IsCompleted || needContinue)
                {
                    needContinue = false;
                    var messages = await queue.GetMessagesAsync(32).ConfigureAwait(false);
                    var cloudQueueMessages = messages as IList<CloudQueueMessage> ?? messages.ToList();
                    if (!cloudQueueMessages.Any())
                    {
                        Console.WriteLine("going to sleep");
                        await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                    }
                    foreach (var message in cloudQueueMessages)
                    {

                        needContinue = true;
                        using (var ms = new MemoryStream(message.AsBytes))
                        {

                            var model = Serializer.Deserialize<SearchModel[]>(ms);
                            if (model.Length > 0)
                            {
                                Console.WriteLine("Inserting a batch");
                                // using (var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(15)))
                                // {

                                foreach (var data in model)
                                {
                                    await client.UpsertDocumentAsync(tableUri, data).ConfigureAwait(false);
                                }

                                //var batch = IndexBatch.MergeOrUpload(model);
                                //Console.WriteLine("upload to search");
                                //try
                                //{
                                //    await indexClient.Documents
                                //        .IndexAsync(batch, cancellationToken: cancellationToken.Token)
                                //        .ConfigureAwait(false);
                                //}
                                //catch (TaskCanceledException)
                                //{
                                //    Console.WriteLine("Canceling index upload");
                                //}
                                //catch (Exception ex)
                                //{
                                //    Console.WriteLine(ex);
                                //}

                                // }
                            }
                        }
                        await queue.DeleteMessageAsync(message).ConfigureAwait(false);
                    }

                }
                Console.WriteLine("Finish processing search");


            });




            list.CollectionChanged += List_CollectionChanged;


            Task.WhenAll(t, t2).Wait();





            void List_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    const int batchSize = 20;
                    if (list.Count > batchSize)
                    {

                        lock (lockObject)
                        {
                            if (list.Count < batchSize)
                            {
                                return;
                            }
                            var amount = batchSize;
                            while (true)
                            {
                                try
                                {
                                    var items = list.Take(amount).ToList();
                                    Serialize(items, queue);
                                    foreach (var item in items)
                                    {
                                        list.Remove(item);
                                        //}
                                    }
                                    break;
                                }
                                catch (ArgumentException ex)
                                {
                                    amount = amount / 2;
                                }
#pragma warning disable CC0004 // Catch block cannot be empty
                                catch (InvalidOperationException ex)
                                {

                                }
#pragma warning restore CC0004 // Catch block cannot be empty
                            }
                        }
                        // }





                    }
                }
            }
        }

        private static void Serialize(IList<SearchModel> items, CloudQueue queue)
        {
            using (var m = new MemoryStream())
            {
                Serializer.Serialize(m, items.Where(w => w != null));
                m.Seek(0, SeekOrigin.Begin);
                queue.AddMessage(new CloudQueueMessage(m.ToArray()));
            }
            Console.WriteLine($"finish uploading a batch {items.Count()} {DateTime.Now}");

        }
    }
}
