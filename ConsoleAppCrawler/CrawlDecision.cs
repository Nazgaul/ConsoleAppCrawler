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
            if (pageToCrawl.Uri.PathAndQuery.Contains("returnUrl"))
            {
                return new CrawlDecision { Allow = false, Reason = "This is redirect Url" };
            }
            return base.ShouldCrawlPage(pageToCrawl, crawlContext);
            //if (!t.Allow)
            //{

            //}
            //return t;
        }


        public CrawlDecision ShouldCrawlSite(SiteToCrawl siteToCrawl)
        {
            return new CrawlDecision { Allow = true };
        }

        public CrawlDecision ShouldRenderJavascript(CrawledPage crawledPage, CrawlContext crawlContext)
        {
            if (crawledPage.HttpWebResponse?.ContentType?.ToLowerInvariant().Contains("text/html") == true)
            {
                return new CrawlDecision { Allow = true };
            }
            return new CrawlDecision { Allow = false, Reason = "not an html page" };
        }
    }
}