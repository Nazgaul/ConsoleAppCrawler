using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace ReadXml
{
    class Program
    {
        private const string EndpointUrl = "https://zboxnew.documents.azure.com:443/";
        private const string PrimaryKey = "y2v1XQ6WIg81Soasz5YBA7R8fAp52XhJJufNmHy1t7y3YQzpBqbgRnlRPlatGhyGegKdsLq0qFChzOkyQVYdLQ==";
        private DocumentClient client;


        static void Main(string[] args)
        {
            //string url = "https://www.wayup.com/guide/feed/";
            //XmlReader readerRss = XmlReader.Create(url);
            //SyndicationFeed feed = SyndicationFeed.Load(readerRss);
            //readerRss.Close();
            //foreach (SyndicationItem item in feed.Items)
            //{
            //    String subject = item.Title.Text;
            //    String summary = item.Summary.Text;

            //}
            Program p = new Program();
            p.GetStartedDemoAsync().Wait();
        }
        // ADD THIS PART TO YOUR CODE
        private async Task GetStartedDemoAsync()
        {
            client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            var file = @"c:\users\ram\downloads\clickcast-jobs.xml";
            var jobs = SimpleStreamAxis(file);
            foreach (var job in jobs)
            {
                var tableUri = UriFactory.CreateDocumentCollectionUri("Zbox", "Jobs");
                await client.UpsertDocumentAsync(tableUri, job).ConfigureAwait(false);
                //await client.CreateDocumentAsync(tableUri, job).ConfigureAwait(false);
            }
        }

        IEnumerable<Job> SimpleStreamAxis(string inputUrl)
        {

            var serializer = new XmlSerializer(typeof(Job));
            using (var reader = XmlReader.Create(inputUrl))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "job")
                        {
                            var el = XNode.ReadFrom(reader) as XElement;
                            if (el != null)
                            {
                                var str = el.ToString();
                                var stringReader = new StringReader(str);
                                var job = (Job)serializer.Deserialize(stringReader);
                                Console.WriteLine(job.Id);
                                yield return job;


                            }
                        }
                    }
                }
            }
        }
    }
}
