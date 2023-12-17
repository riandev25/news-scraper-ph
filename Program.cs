//using ExtractNews.EfCore.Context;
//using ExtractNews.EfCore.Models;
//using ExtractNews.Helper;
//using ExtractNews.Model;
//using Microsoft.Extensions.Configuration;
//using news_scraper_ph.Helper;
//using PuppeteerSharp;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//public class NewsScraper
//{
//    private readonly List<string> _urlPaths;
//    private readonly LaunchOptions _puppeteerOptions;
//    private readonly DbHelper _db;

//    public NewsScraper(List<string> urlPaths, NewsDbContext newsDbContext)
//    {
//        _urlPaths = urlPaths;

//        _puppeteerOptions = new LaunchOptions()
//        {
//            Headless = false,
//            ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
//            DefaultViewport = null
//        };
//        _db = new DbHelper(newsDbContext);
//    }

//    public async Task Run()
//    {
//        try
//        {  
//            // start month input handler
//            Console.WriteLine("Month start (ex. november):");
//            string? inputMonthStart = Console.ReadLine();
//            ScriptInputsHelper.ThrowIfNullOrEmpty(inputMonthStart, "Didn't provide month start to scrape");

//            // start year input handler
//            Console.WriteLine("Year start (ex. 2023):");
//            string inputYearStart = Console.ReadLine();
//            ScriptInputsHelper.ThrowIfNullOrEmpty(inputYearStart, "Didn't provide year start to scrap");

//            // end month input handler
//            Console.WriteLine("Month end (ex. december):");
//            string inputMonthEnd = Console.ReadLine();
//            ScriptInputsHelper.ThrowIfNullOrEmpty(inputMonthEnd, "Didn't provide month end to scrape");

//            // end date
//            Console.WriteLine("Year end (ex. 2023):");
//            string inputYearEnd = Console.ReadLine();
//            ScriptInputsHelper.ThrowIfNullOrEmpty(inputYearEnd, "Didn't provide year to scrape");

//            List<string> keywordsToKeep = [];
//            Console.WriteLine("Enter section/s to scrape (separate by a comma):");
//            Console.WriteLine("topstories");
//            Console.WriteLine("money");
//            Console.WriteLine("sports");
//            Console.WriteLine("pinoyabroad");
//            Console.WriteLine("sci-tech");
//            Console.WriteLine("showbiz");
//            Console.WriteLine("lifestyle");
//            Console.WriteLine("opinion");
//            Console.WriteLine("hashtag");
//            Console.WriteLine("serbisyopubliko");
//            Console.WriteLine("cbb");
//            Console.WriteLine("For example: topstories,money");

//            string inputSectionsToScrape = Console.ReadLine();
//            char[] separators = [',', ' '];
//            ScriptInputsHelper.ThrowIfNullOrEmpty(inputSectionsToScrape, "Didn't provide sections to scrape");

//            // Split the input string by the defined separators
//            string[] inputSectionArray = inputSectionsToScrape.Split(separators, StringSplitOptions.RemoveEmptyEntries);
//            keywordsToKeep.AddRange(inputSectionArray);

//            using var browser = await Puppeteer.LaunchAsync(_puppeteerOptions);
//            using var page = await browser.NewPageAsync();

//            var filteredUrlPaths = _urlPaths
//                .Where(section => keywordsToKeep.Any(keyword => section.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
//                .ToArray();

//            List<TitleUrlModel> href = [];

//            foreach (var urlPath in filteredUrlPaths)
//            {
//                var scrapedData = await ScrapeNewsFromAndToHelper.ScrapeNewsFromAndTo(page, urlPath, "december", 2023, "december", 2023);
//                href.AddRange(scrapedData);
//            }

//            foreach (var item in href)
//            {
//                Console.WriteLine($"Title: {item.Title}, NewsUrl: {item.NewsUrl}, ImageUrl: {item.ImageUrl}, SubSection: {item.SubSection}, DatetimeUploaded: {item.DateTimeUploaded}");
//                item.NewsUrl = new Uri(item.NewsUrl.ToString());
//                item.ImageUrl = new Uri(item.ImageUrl.ToString());
//            }
//            _db.SaveOrder(href);        
//        }
//        catch (Exception e)
//        {
//            Console.WriteLine($"Error: {e}");
//        }
//    }
//}


//public class Program
//{
//    public static async Task Main(string[] args)
//    {
//        List<string> urlPaths =
//        [
//            "topstories", "money", "sports", "pinoyabroad", "sci-tech", "showbiz", "lifestyle", "opinion", "hashtag", "serbisyopubliko", "cbb"
//        ];
//        using var dbContext = new NewsDbContext();
//        NewsScraper newsScraper = new(urlPaths, dbContext);
//        await newsScraper.Run();
//    }
//}

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

            List<TitleUrlModel> href = [];

            foreach (var urlPath in filteredUrlPaths)
            {
                var scrapedData = await ScrapeNewsFromAndToHelper.ScrapeNewsFromAndTo(page, urlPath, "december", 2023, "december", 2023);
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



