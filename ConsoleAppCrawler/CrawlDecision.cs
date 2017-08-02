using System.Linq;
using Abot.Core;
using Abot.Poco;
using AbotX.Core;
using AbotX.Poco;

namespace ConsoleAppCrawler
{
    public class CrawlDecisionSpitball : CrawlDecisionMaker, ICrawlDecisionMakerX
    {
        public override CrawlDecision ShouldCrawlPage(PageToCrawl pageToCrawl, CrawlContext crawlContext)
        {
            //if (pageToCrawl.Uri.Authority.Equals("spitball.co"))
            //{
            //    if (pageToCrawl.Uri.PathAndQuery.Contains("returnUrl"))
            //    {
            //        return new CrawlDecision {Allow = false, Reason = "This is redirect Url"};
            //    }
            //}
            if (pageToCrawl.Uri.PathAndQuery.ToLowerInvariant().Contains("xml"))
                return new CrawlDecision
                {
                    Allow = true
                };
            if (pageToCrawl.Uri.Authority.Equals("studysoup.com"))
            {
                var segments = pageToCrawl.Uri.Segments;

                if (segments.Length < 2)
                    return new CrawlDecision {Allow = false, Reason = "Not Part of studysoup white list"};
                var absPath = pageToCrawl.Uri.Segments[1].ToLowerInvariant().Replace("/", string.Empty);
                var whiteList = new[] {"flashcard", "note", "guide", "bundle"};

                if (whiteList.Contains(absPath))
                    return base.ShouldCrawlPage(pageToCrawl, crawlContext);
                return new CrawlDecision {Allow = false, Reason = "Not Part of studysoup white list"};
            }
            else if (pageToCrawl.Uri.Authority.Equals("quizlet.com"))
            {
                var segments = pageToCrawl.Uri.Segments;
                var blackList = new[] {"flashcards", "learn", "spell", "match", "test", "gravity"};
                if (segments.Length >= 3&& blackList.Contains(segments[2].ToLowerInvariant().Replace("/", string.Empty)))
                    return new CrawlDecision { Allow = false, Reason = "Not neef in mapping" };
                //var absPath = pageToCrawl.Uri.Segments[1].ToLowerInvariant().Replace("/", string.Empty);
                //if (int.TryParse(absPath,out int quizNum))
                //    return base.ShouldCrawlPage(pageToCrawl, crawlContext);
                //return new CrawlDecision { Allow = false, Reason = "Not Part of Quizlet white list" };
            }
            if (pageToCrawl.Uri.Authority.Equals("www.khanacademy.org"))
            {
                var segments = pageToCrawl.Uri.Segments;

                if (segments.Length < 4)
                    return new CrawlDecision {Allow = false, Reason = "Not Part of course hero white list"};
                var absPath = pageToCrawl.Uri.Segments[1].ToLowerInvariant().Replace("/", string.Empty);
                var secondSeg = pageToCrawl.Uri.Segments[2].ToLowerInvariant().Replace("/", string.Empty);
                //Ignore math of high-school class
                var mathSchool = new[] {"early-math", "arithmetic"}.Contains(secondSeg) ||
                                 secondSeg.StartsWith("cc-") ||
                                 secondSeg.Contains("-grade") || secondSeg.Contains("engageny") ||
                                 secondSeg.Contains("-engage-");
                var whiteList = new[] {"math", "science", "computing", "humanities", "economics-finance-domain"};
                if (whiteList.Contains(absPath) && (!absPath.Equals("math") || !mathSchool))
                    return base.ShouldCrawlPage(pageToCrawl, crawlContext);
                return new CrawlDecision {Allow = false, Reason = "Not Part of Khanan white list"};
            }
            if (pageToCrawl.Uri.Authority.Equals("www.coursehero.com"))
            {
                if (pageToCrawl.Uri.PathAndQuery.ToLowerInvariant().Contains("xml"))
                    return new CrawlDecision
                    {
                        Allow = true
                    };
                var segments = pageToCrawl.Uri.Segments;

                if (segments.Length < 2)
                    return new CrawlDecision {Allow = false, Reason = "Not Part of course hero white list"};
                var absPath = pageToCrawl.Uri.Segments[1].ToLowerInvariant().Replace("/", string.Empty);
                var whiteList = new[] {"flashcard", "file"};

                if (whiteList.Contains(absPath))
                    return base.ShouldCrawlPage(pageToCrawl, crawlContext);
                return new CrawlDecision {Allow = false, Reason = "Not Part of studysoup white list"};
            }
            return base.ShouldCrawlPage(pageToCrawl, crawlContext);
        }


        public CrawlDecision ShouldCrawlSite(SiteToCrawl siteToCrawl)
        {
            return new CrawlDecision {Allow = true};
        }

        public CrawlDecision ShouldRenderJavascript(CrawledPage crawledPage, CrawlContext crawlContext)
        {
            if (crawledPage.HttpWebResponse?.ContentType?.ToLowerInvariant().Contains("text/html") == true)
                return new CrawlDecision {Allow = true};
            return new CrawlDecision {Allow = false, Reason = "not an html page"};
        }
    }
}