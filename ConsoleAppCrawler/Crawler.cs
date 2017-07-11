using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Abot.Core;
using Abot.Poco;
using AbotX.Core;
using AbotX.Crawler;
using AbotX.Poco;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace ConsoleAppCrawler
{
    public class Crawler
    {
        private readonly CrawlerX m_Crawler;
        private readonly CrawlDecisionSpitball m_CrawlDecision = new CrawlDecisionSpitball();
        private readonly ObservableCollection<SearchModel> m_List;
        private readonly object m_LockObject;
        private int m_Count;
        private int m_Count2;
        public Crawler(ObservableCollection<SearchModel> collection, object obj)
        {
            var finder = new SiteMapFinder();
            var config = AbotXConfigurationSectionHandler.LoadFromXml().Convert();

            var implementation = new ImplementationOverride(config, new ImplementationContainer
            {
                HyperlinkParser = finder,
                CrawlDecisionMakerX = m_CrawlDecision,
                CrawlDecisionMaker = m_CrawlDecision
            });
            m_Crawler = new CrawlerX(config, implementation);
            m_Crawler.PageCrawlCompleted += crawler_ProcessPageCrawlCompleted;
            m_Crawler.PageCrawlDisallowed += Crawler_PageCrawlDisallowed;
            m_Crawler.PageCrawlStarting += M_Crawler_PageCrawlStarting;
            m_Crawler.ShouldDownloadPageContent((page, context) =>
            {
                return new CrawlDecision { Allow = true };
            });

            m_List = collection;
            m_LockObject = obj;
        }



        private void M_Crawler_PageCrawlStarting(object sender, Abot.Crawler.PageCrawlStartingArgs e)
        {
            m_Count2++;
        }

        private void Crawler_PageCrawlDisallowed(object sender, Abot.Crawler.PageCrawlDisallowedArgs e)
        {
            // Console.WriteLine("disallow page");
        }

        public async Task<CrawlResult> DoStuffAsync()
        {
            //https://www.spitball.co/sitemap.xml
           // var t = await m_Crawler.CrawlAsync(new Uri("https://studysoup.com/sitemap.xml.gz")).ConfigureAwait(false);
            var t = await m_Crawler.CrawlAsync(new Uri("https://studysoup.com/note/2413595/cnc-142-week-11-spring-2017-kasandra-angermeier")).ConfigureAwait(false);
            t = await m_Crawler.CrawlAsync(new Uri("https://studysoup.com/guide/2392731/cnc-142-midterm-spring-2017-kasandra-angermeier")).ConfigureAwait(false);
            t = await m_Crawler.CrawlAsync(new Uri("https://studysoup.com/flashcard/272449/micro-economics-taya-schowalter-iv")).ConfigureAwait(false);
            t = await m_Crawler.CrawlAsync(new Uri("https://studysoup.com/bundle/2310235/buad-341-001-week-1-fall-2016-randi-myers")).ConfigureAwait(false);
            Console.WriteLine("finish to crawl with count " + m_Count);
            //Console.WriteLine($"finish to crawl with count {crawlDecision.count} {m_Count2}");

            return t;
            //var result =
            //    crawler.Crawl(new Uri(
            //        "https://www.spitball.co/item/nyu-new-york-university/138206/machine-design/521071/lecture-ch-11-591.pdf/"));

            //if (result.ErrorOccurred)
            //    Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
            //else
            //    Console.WriteLine("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);
        }


        void crawler_ProcessPageCrawlCompleted(object sender, Abot.Crawler.PageCrawlCompletedArgs e)
        {
            var crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
                return;
            }

            if (!crawledPage.HttpWebResponse.ContentType.Contains("text/html"))
            {
                return;

            }

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
            {
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
                return;
            }
            Object model;
            if (crawledPage.Uri.Authority.Equals("studysoup.com"))
            {
                model = CreateStudySoupNote(crawledPage);
            }
            //m_Count++;

            //var title = angleSharpHtmlDocument.Title;
            //var metaElement = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[name='description']");
            //var t = angleSharpHtmlDocument.QuerySelector(".text-wrapper>.text");
            //var content = t?.Text()?.Trim();

            //var meta = metaElement?.Content?.Trim();

            //var idStr = Uri.UnescapeDataString(crawledPage.Uri.AbsoluteUri);

            //var id = Base64UrlEncoder.Encode(idStr);
            //var model = new SearchModel
            //{
            //    Id = id, //.Segments[5].TrimEnd('/'),
            //    Title = title,
            //    Content = content,
            //    Meta = meta,
            //    Url = idStr
            //};
            //lock (m_LockObject)
            //{
            //    //m_List.Add(model);
            //}

        }

        Object CreateStudySoupNote(CrawledPage crawledPage)
        {
            var angleSharpHtmlDocument = crawledPage.AngleSharpHtmlDocument; //AngleSharp parser

            var viewsText = angleSharpHtmlDocument.QuerySelector(".document-metrics:nth-child(3)")?.Text()?.Trim();
            var views = (!String.IsNullOrEmpty(viewsText)) ? Regex.Match(viewsText, @"\d+").Value : "0";
            var title = angleSharpHtmlDocument.QuerySelector("span.current")?.Text()?.Trim();
            var metaDescription = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[name=description]")?.Content?.Trim();
            var metaTitle = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[name=title]")?.Content?.Trim();
            var metaKeyword = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[name=keywords]")?.Content?.Trim();
            var metaImage = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[property='og:image']")?.Content?.Trim();
            string university=String.Empty, course=String.Empty, content,tags=String.Empty;
            string extraData = angleSharpHtmlDocument.QuerySelector(".small-padding-bottom.detail-box")?.Text();
            if (String.IsNullOrEmpty(extraData))
            {
                if (extraData.IndexOf("School:") > -1)
                    university = extraData.Substring(extraData.IndexOf("School:") + "School:".Length).Split('\n')[0]?.Trim();
                if (extraData.IndexOf("Course:") > -1)
                    course = extraData.Substring(extraData.IndexOf("Course:") + "Course:".Length).Split('\n')[0]?.Trim();
                if (extraData.IndexOf("Tags:") > -1)
                    tags = extraData.Substring(extraData.IndexOf("Tags:") + "Tags:".Length).Split('\n')[0]?.Trim();
            }
            if (!crawledPage.Uri.AbsolutePath.StartsWith("/flashcard"))
            {
                content = angleSharpHtmlDocument.QuerySelector("#material_text").Text()?.Trim();
            }
            else {
                //Remove the cards front back headers from the text
                var allContent = angleSharpHtmlDocument.QuerySelector("#preview")?.Text();
                var backIndex = allContent.IndexOf("Back");
                content = allContent.Substring(backIndex + "Back".Length)?.Trim();
            }
            return new { university, course, title, content, views, metaTitle, metaDescription, metaKeyword, tags};
        }
    }
}
