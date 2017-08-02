using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;
using AbotX.Core;
using AbotX.Crawler;
using AbotX.Poco;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using System.Linq;

namespace ConsoleAppCrawler
{
    public class Crawler
    {
        private readonly CrawlDecisionSpitball m_CrawlDecision = new CrawlDecisionSpitball();
        private readonly CrawlerX m_Crawler;
        private readonly ObservableCollection<CrawlModel> m_List;
        private readonly object m_LockObject;
        private int count = 0;
        public Crawler(ObservableCollection<CrawlModel> collection, object obj)
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
            //m_Crawler.PageCrawlDisallowed += Crawler_PageCrawlDisallowed;
            //m_Crawler.PageCrawlStarting += M_Crawler_PageCrawlStarting;
            ////m_Crawler.ShouldDownloadPageContent((page, context) =>
            ////{
            ////    return new CrawlDecision { Allow = true };
            ////});

            m_List = collection;
            m_LockObject = obj;
        }

        public async Task<CrawlResult> DoStuffAsync()
        {
            //https://www.spitball.co/sitemap.xml
            // var t = await m_Crawler.CrawlAsync(new Uri("https://studysoup.com/sitemap.xml.gz")).ConfigureAwait(false);
            var t = await m_Crawler
                .CrawlAsync(new Uri("https://www.studyblue.com/online-flashcards")).ConfigureAwait(false);
            //var t = await m_Crawler.CrawlAsync(new Uri("https://www.khanacademy.org/math/algebra/rational-and-irrational-numbers/proofs-concerning-irrational-numbers/v/proof-that-square-root-of-2-is-irrational")).ConfigureAwait(false);
            //t = await m_Crawler.CrawlAsync(new Uri("https://studysoup.com/flashcard/272449/micro-economics-taya-schowalter-iv")).ConfigureAwait(false);
            //t = await m_Crawler.CrawlAsync(new Uri("https://studysoup.com/bundle/2310235/buad-341-001-week-1-fall-2016-randi-myers")).ConfigureAwait(false);
            //Console.WriteLine("finish to crawl with count " + m_Count);
            //Console.WriteLine($"finish to crawl with count {crawlDecision.count} {m_Count2}");
            Console.WriteLine("count is {0}", count);
            return t;
            //var result =
            //    crawler.Crawl(new Uri(
            //        "https://www.spitball.co/item/nyu-new-york-university/138206/machine-design/521071/lecture-ch-11-591.pdf/"));

            //if (result.ErrorOccurred)
            //    Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
            //else
            //    Console.WriteLine("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);
        }


        private void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            var crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
                return;
            }

            if (!crawledPage.HttpWebResponse.ContentType.Contains("text/html"))
                return;
            var validAuth = new Dictionary<string, string>
            {
                {"studysoup.com", "CreateStudySoupNote"},
                {"www.khanacademy.org", "CreateKhananNote"},
                { "quizlet.com","CreateQuizletFlashcard"},
                { "www.studyblue.com","CreateStudyBlueFlashcard"}
            };
            if (validAuth.TryGetValue(crawledPage.Uri.Authority, out string modelFunction))
            {
                var model = (CrawlModel) GetType()
                    .GetMethod(modelFunction, BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, new[] {crawledPage});
                if (model != null)
                {
                    count++;
                    //Console.WriteLine("Finish processing {0}", crawledPage.Uri.AbsoluteUri);
                }
                //if (count % 50 == 0)
                //{
                //    Console.WriteLine("count is {0}", count);
                //}
            }
            if (string.IsNullOrEmpty(crawledPage.Content.Text))
            {
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
                return;
            }
            //if (crawledPage.Uri.Authority.Equals("studysoup.com"))
            //{
            //    var model = CreateStudySoupNote(crawledPage);
            //    Console.WriteLine("Finish processing {0}", crawledPage.Uri.AbsoluteUri);
            //    lock (m_LockObject)
            //    {
            //        m_List.Add(model);
            //    }
            //}
            //else if (crawledPage.Uri.Authority.Contains("www.khanacademy.org"))
            //{
            //    var model = (CrawlModel) GetType()
            //        .GetMethod("CreateKhananNote", BindingFlags.Static | BindingFlags.NonPublic)
            //        .Invoke(null, new[] {crawledPage}); //CreateKhananNote(crawledPage);
            //    Console.WriteLine("Finish processing {0}, image: {1}, content: {2}", crawledPage.Uri.AbsoluteUri,
            //        model.Image, model.Content);
            //    lock (m_LockObject)
            //    {
            //        m_List.Add(model);
            //    }
            //}
            //else if (crawledPage.Uri.Authority.Equals("www.studyblue.com"))
            //{
            //    CrawlModel model;
            //    var pathVal = crawledPage.Uri.AbsoluteUri.Substring(crawledPage.Uri.AbsoluteUri.IndexOf(".com") + 4);
            //    if (pathVal.StartsWith("/#flashcard/view"))
            //        model = CreateStudyBlueFlashcard(crawledPage);
            //    else
            //        model = CreateCourseHeroNote(crawledPage);
            //    Console.WriteLine("Finish processing {0}", crawledPage.Uri.AbsoluteUri);
            //    lock (m_LockObject)
            //    {
            //        m_List.Add(model);
            //    }
            //}
            //else if (crawledPage.Uri.Authority.Equals("www.coursehero.com"))
            //{
            //    CrawlModel model;
            //    if (crawledPage.Uri.AbsolutePath.StartsWith("/flashcard"))
            //        model = CreateCourseHeroFlashcard(crawledPage);
            //    else
            //        model = CreateCourseHeroNote(crawledPage);
            //    Console.WriteLine("Finish processing {0}", crawledPage.Uri.AbsoluteUri);
            //    lock (m_LockObject)
            //    {
            //        m_List.Add(model);
            //    }
            //}
            //m_Count++;
        }

        private static string CalculateMd5Hash(string input)

        {
            // step 1, calculate MD5 hash from input

            var md5 = MD5.Create();

            var inputBytes = Encoding.ASCII.GetBytes(input);

            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            var sb = new StringBuilder();

            for (var i = 0; i < hash.Length; i++)

                sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }

        private static CrawlModel CreateQuizletFlashcard(CrawledPage page)
        {
            var doc = page.AngleSharpHtmlDocument;
            var metaDescription = doc.QuerySelector<IHtmlMetaElement>("meta[name=description]")?.Content?.Trim();
            var allCards=doc.QuerySelectorAll(".SetPage-term").Select(s => string.IsNullOrEmpty(s?.TextContent)
                ? string.Empty
                : Regex.Replace(s?.TextContent?.Trim(), @"([a-z])([A-Z])", "$1 $2", RegexOptions.CultureInvariant));
            var enumerable = allCards as string[] ?? allCards.ToArray();
            if (!enumerable.Any())
                return null;
            var list = enumerable?.ToList();
            var firstDefImage = doc.QuerySelector(".SetPageTerm-image")?.GetAttribute("style");
            var logoImg = doc.QuerySelector("img")?.GetAttribute("src");
            var image = !(string.IsNullOrEmpty(firstDefImage))?Regex.Match(firstDefImage, @"(?<=\().+?(?=\))").Value:
                (!string.IsNullOrEmpty(logoImg) ? (page.ParentUri.OriginalString + logoImg):null);
            list.RemoveAll(string.IsNullOrWhiteSpace);
            var content = list.Count>0?list.Aggregate<string>((a, b) => a + ", " + b):null;
            return new CrawlModel(page.Uri.AbsoluteUri, page.Uri.Host, doc.Title, content, metaDescription,image,
                GetMd5Hash(MD5.Create(), page.Uri.AbsoluteUri));
        }
        private static CrawlModel CreateKhananNote(CrawledPage page)
        {
            var doc = page.AngleSharpHtmlDocument;
            var metaDescription = doc.QuerySelector<IHtmlMetaElement>("meta[name=description]")?.Content?.Trim();
            var metaImage = doc.QuerySelector("[rel=image_src]")?.GetAttribute("href") ??
                            doc.QuerySelector<IHtmlMetaElement>("meta[name=thumbnail]")?.Content?.Trim() ??
                            doc.QuerySelector("[role=tabpanel] .fixed-to-responsive img")?.GetAttribute("src") ??
                            doc.QuerySelector<IHtmlMetaElement>("meta[property='og:image']")?.Content?.Trim();
            var metaKeyword = doc.QuerySelector<IHtmlMetaElement>("meta[name=keywords]")?.Content?.Trim()
                .Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries);
            var tags = doc.QuerySelector<IHtmlMetaElement>("meta[name='sailthru.tags']")?.Content?.Trim()
                .Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries);
            //Check if the image is cdn not exist change to default
            if (metaImage != null &&
                metaImage.Equals(
                    "https://cdn.kastatic.org/googleusercontent/K5bzbA067FpSFjs7VuTCAEosuCGLm4NfxQbq_tYtpMHIyB5j-nirP_Pdy8XXrmoARE3_2TBnGafYaRTsSiFt4iw")
            )
                metaImage = doc.QuerySelector<IHtmlMetaElement>("meta[property='og:image']")?.Content?.Trim();
            var content = doc.QuerySelector(".perseus-renderer")?.Text()?.Trim() ?? doc
                              .QuerySelector(".task-container:not([itemtype*=Video]),[class^=tabContent]")?.Text()
                              ?.Trim() ??
                          doc.QuerySelector("[class^=description],[class^=module]")?.TextContent?.Trim();
            //split in case lowerCase close to UpperCase
            if (!string.IsNullOrEmpty(content)) content = Regex.Replace(content, @"([a-z])([A-Z])", "$1 $2");
            var type = ItemType.Document;
            return new CrawlModel(page.Uri.AbsoluteUri, doc.Title, content, null, null,
                tags, null, null, metaDescription, metaImage, metaKeyword, page.Uri.Host,
                GetMd5Hash(MD5.Create(), page.Uri.AbsoluteUri), type);
        }

        private static CrawlModel CreateStudyBlueFlashcard(CrawledPage page)
        {
            var doc = page.AngleSharpHtmlDocument;
            var metaDescription = doc.QuerySelector<IHtmlMetaElement>("meta[name=description]")?.Content?.Trim();
            var allCards = doc.QuerySelectorAll("[id^=card].card").Select(s => string.IsNullOrEmpty(s?.TextContent)
                ? string.Empty
                : s?.TextContent?.Trim());
            var enumerable = allCards as string[] ?? allCards.ToArray();
            if (!enumerable.Any())
                return null;
            var list = enumerable?.ToList();
            var metaImg = doc.QuerySelector<IHtmlMetaElement>("meta[property='og:image']")?.Content?.Trim();
            list.RemoveAll(string.IsNullOrWhiteSpace);
            //Remove extra spaces 
            var spaceReg = new Regex(@"\s+", RegexOptions.Compiled);
            var content = list.Count > 0 ? spaceReg.Replace(list.Aggregate((a, b) => a + ", " + b), " ") : null;
            return new CrawlModel(page.Uri.AbsoluteUri, page.Uri.Host, doc.Title, content, metaDescription, metaImg,
                GetMd5Hash(MD5.Create(), page.Uri.AbsoluteUri));
        }

        private static CrawlModel CreateCourseHeroFlashcard(CrawledPage page)
        {
            var doc = page.AngleSharpHtmlDocument;
            var university = doc.QuerySelector("[itemprop=educationalAlignment] span")?.Text()?.Trim();
            var course = doc.QuerySelector("[itemprop=isPartOf] span")?.Text()?.Trim();
            var metaDescription = doc.QuerySelector<IHtmlMetaElement>("meta[name=description]")?.Content?.Trim();
            var metaImage = doc.QuerySelector("main img")?.GetAttribute("src") ??
                            doc.QuerySelector<IHtmlMetaElement>("meta[property='og:image']")?.Content?.Trim();
            var content = doc.QuerySelector(".html-preview-text")?.Text()?.Trim() ??
                          doc.QuerySelector(".bdp_preview_paragraph[itemprop=text] span")?.Text()?.Trim();
            var tagsText = doc.QuerySelector("[itemprop=keywords]")?.TextContent?.Replace("  ", "")?.Replace("\n", "")
                ?.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            var type = ItemType.Flashcard;

            int? views = null;
            if (int.TryParse(doc.QuerySelector(".ch_docLandingStats_single h2")?.Text()?.Trim(), out int realViews))
                views = realViews;

            return null;
            //return CrawlModel(page.Uri.AbsoluteUri, doc.Title, content, university, course,
            //tags, createDate, , metaDescription, metaImage, , page.Uri.Host, GetMd5Hash(MD5.Create(), (page.Uri.AbsoluteUri)), type);
        }

        private static CrawlModel CreateCourseHeroNote(CrawledPage page)
        {
            var doc = page.AngleSharpHtmlDocument;
            var university = doc.QuerySelector("[itemprop=educationalAlignment] span")?.Text()?.Trim();
            var course = doc.QuerySelector("[itemprop=isPartOf] span")?.Text()?.Trim();
            var metaDescription = doc.QuerySelector<IHtmlMetaElement>("meta[name=description]")?.Content?.Trim();
            var metaImage = doc.QuerySelector("main img")?.GetAttribute("src") ??
                            doc.QuerySelector<IHtmlMetaElement>("meta[property='og:image']")?.Content?.Trim();
            var content = doc.QuerySelector(".html-preview-text")?.Text()?.Trim() ??
                          doc.QuerySelector(".bdp_preview_paragraph[itemprop=text] span")?.Text()?.Trim();
            var tagsText = doc.QuerySelector("[itemprop=keywords]")?.TextContent?.Replace("  ", "")?.Replace("\n", "")
                ?.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            var type = ItemType.Document;
            var datesMap =
                new Dictionary<string, DateTime>();
            datesMap.Add("fall", new DateTime(2000, 8, 22));
            datesMap.Add("spring", new DateTime(2000, 1, 9));
            datesMap.Add("summer", new DateTime(2000, 5, 22));
            int? views = null;
            if (int.TryParse(doc.QuerySelector(".ch_docLandingStats_single h2")?.Text()?.Trim(), out int realViews))
                views = realViews;
            var c = 12;
            c = 7;
            return null;
            //return CrawlModel(page.Uri.AbsoluteUri, doc.Title, content, university, course,
            //tags, createDate, , metaDescription, metaImage, , page.Uri.Host, GetMd5Hash(MD5.Create(), (page.Uri.AbsoluteUri)), type);
        }

        private static CrawlModel CreateStudySoupNote(CrawledPage page)
        {
            var angleSharpHtmlDocument = page.AngleSharpHtmlDocument;
            int? views = null;
            var viewsText = angleSharpHtmlDocument.QuerySelector(".document-metrics:nth-child(3)")?.Text();
            if (!string.IsNullOrEmpty(viewsText))
                if (int.TryParse(Regex.Match(viewsText, @"\d+").Value, out int realViews))
                    views = realViews;
            //var contentCountText = angleSharpHtmlDocument.QuerySelector("span.document-metrics")?.Text()?.Trim();
            //int.TryParse(Regex.Match(contentCountText ?? "", @"\d+").Value, out int contentCount);
            //var title = angleSharpHtmlDocument.QuerySelector("span.current")?.Text()?.Trim() ?? angleSharpHtmlDocument.QuerySelector("title")?.Text()?.Trim();

            var metaDescription = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[name=description]")
                ?.Content?.Trim();
            var metaKeyword = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[name=keywords]")?.Content
                ?.Trim().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var metaImage = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[property='og:image']")?.Content
                ?.Trim();
            string university = null, course = null, content = null;
            string[] tags = { };
            var type = ItemType.Document;
            var allHeaders = angleSharpHtmlDocument.QuerySelectorAll(".small-padding-bottom.detail-box h5");
            DateTime? createDate = null;
            //Init the extra data details according what exist
            foreach (var item in allHeaders)
            {
                var header = item.ChildNodes[0].Text().Trim();
                if (header.Equals("School:"))
                {
                    university = item.FirstElementChild.Text();
                }
                else if (header.Equals("Course:"))
                {
                    course = item.FirstElementChild.Text();
                }
                else if (header.Equals("Tags:"))
                {
                    tags = item.FirstElementChild.Text()?.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                }
                else if (header.Equals("Upload Date:"))
                {
                    var dateString = item.FirstElementChild.Text().Replace("  ", " ");
                    if (DateTime.TryParseExact(dateString, "ddd MMM d HH:mm:ss yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime realCreateDate))
                        createDate = realCreateDate;
                }
            }

            if (!page.Uri.AbsolutePath.StartsWith("/flashcard"))
            {
                var contentSplit = angleSharpHtmlDocument.QuerySelector("#material_text")?.TextContent?.Split('\n');
                content = contentSplit.Length > 3 ? contentSplit[2]?.Trim() : contentSplit[0]?.Trim();
                // var contentSelector = angleSharpHtmlDocument.QuerySelector("#pageContainer1");
                // var canvas= angleSharpHtmlDocument.QuerySelector<IHtmlCanvasElement>("#page1");
                // var htmlToImageConv = new HtmlToImageConverter();
                // var image=htmlToImageConv.GenerateImage(contentSelector.OuterHtml, ImageFormat.Jpeg);
                // //Image x = (Bitmap)(new ImageConverter().ConvertFrom(image));
                //// x.Save("yifat.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                // if (canvas != null)
                // {
                //     var ctx = canvas.GetContext("2d");
                //     var c = 5;
                // }

                var firstPageImg = angleSharpHtmlDocument.QuerySelector(".page_number_1");
                if (firstPageImg != null)
                {
                    var index = firstPageImg.GetAttribute("style").IndexOf("url(");
                    metaImage = firstPageImg.OuterHtml.Substring(index + 4).Split(')')[0];
                }
            }
            else
            {
                type = ItemType.Flashcard;
                var spaceReg = new Regex(@"\s+", RegexOptions.Compiled);
                //Remove the cards front back headers from the text
                var slides = angleSharpHtmlDocument.QuerySelectorAll<IHtmlDivElement>("#preview>div:first-child .row");
                if (slides != null)
                {
                    var builder = new StringBuilder();
                    foreach (var slide in slides)
                    {
                        if (slide.ClassList.Length > 1)
                            continue;
                        var txt = spaceReg.Replace(slide.Text(), " ");
                        builder.Append(txt);
                    }
                    content = builder.ToString();
                }
                //if (allContent != null)
                //{
                //    var backIndex = allContent.IndexOf("Back", StringComparison.Ordinal);
                //    content = allContent.Substring(backIndex + "Back".Length).Trim();
                //}
            }
            return new CrawlModel(page.Uri.AbsoluteUri, angleSharpHtmlDocument.Title, content, university, course,
                tags, createDate, views, metaDescription, metaImage, metaKeyword, page.Uri.Host,
                GetMd5Hash(MD5.Create(), page.Uri.AbsoluteUri), type);
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (var i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}