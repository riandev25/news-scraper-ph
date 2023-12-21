using ExtractNews.EfCore.Context;
using ExtractNews.Helper;
using ExtractNews.Model;
using news_scraper_ph.Helper;
using PuppeteerSharp;

public class NewsScraper
{
    private readonly LaunchOptions _puppeteerOptions;
    private readonly DbHelper _db;

    public NewsScraper(NewsDbContext newsDbContext)
    {

        _puppeteerOptions = new LaunchOptions()
        {
            Headless = false,
            ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
            DefaultViewport = null
        };
        _db = new DbHelper(newsDbContext);
    }

    public async Task Run()
    {
        try
        {
            var (inputMonthStart, inputYearStart, inputMonthEnd, inputYearEnd) = ScriptInputsHelper.GetValidInputsForDateScrapeRange();
            var filteredUrlPaths = ScriptInputsHelper.GetFilteredUrlPaths();

            using var browser = await Puppeteer.LaunchAsync(_puppeteerOptions);
            using var page = await browser.NewPageAsync();

            List<RawNewsModel> href = [];

            foreach (var urlPath in filteredUrlPaths)
            {
                var scrapedData = await ScrapeNewsFromAndToHelper.ScrapeNewsFromAndTo(page, urlPath, inputMonthStart, inputYearStart, inputMonthEnd, inputYearEnd);
                href.AddRange(scrapedData);
            }

            foreach (var item in href)
            {
                Console.WriteLine($"Title: {item.Title}, NewsUrl: {item.NewsUrl}, ImageUrl: {item.ImageUrl}, SubSection: {item.SubSection}, DatetimeUploaded: {item.DateTimeUploaded}");
                item.NewsUrl = new Uri(item.NewsUrl.ToString());
                item.ImageUrl = new Uri(item.ImageUrl.ToString());
            }
            _db.SaveOrder(href);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
        }
    }
}


public class Program
{
    public static async Task Main(string[] args)
    {
        using var dbContext = new NewsDbContext();
        NewsScraper newsScraper = new(dbContext);
        await newsScraper.Run();
    }
}



