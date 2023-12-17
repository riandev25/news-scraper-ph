namespace news_scraper_ph.Helper
{
    public static class ScriptInputsHelper
    {
        public static (string?, string?, string?, string?) GetValidInputsForDateScrapeRange()
        {
            // start month input handler
            Console.WriteLine("Month start (ex. november):");
            string? inputMonthStart = Console.ReadLine();
            string validInputMonthStart = ThrowIfNullOrEmptyAndReturnIfNot(inputMonthStart, "Didn't provide month start to scrape");

            // start year input handler
            Console.WriteLine("Year start (ex. 2023):");
            string inputYearStart = Console.ReadLine();
            string validInputYearStart = ThrowIfNullOrEmptyAndReturnIfNot(inputYearStart, "Didn't provide year start to scrap");

            // end month input handler
            Console.WriteLine("Month end (ex. december):");
            string inputMonthEnd = Console.ReadLine();
            string validInputMonthEnd = ThrowIfNullOrEmptyAndReturnIfNot(inputMonthEnd, "Didn't provide month end to scrape");

            // end date input handler
            Console.WriteLine("Year end (ex. 2023):");
            string inputYearEnd = Console.ReadLine();
            string validInputYearEnd = ThrowIfNullOrEmptyAndReturnIfNot(inputYearEnd, "Didn't provide year to scrape");
            return (validInputMonthStart, validInputYearStart, validInputMonthEnd, validInputYearEnd);
        }

        public static List<string> GetFilteredUrlPaths()
        {
            List<string> urlPaths =
            [
                "topstories",
                "money",
                "sports",
                "pinoyabroad",
                "sci-tech",
                "showbiz",
                "lifestyle",
                "opinion",
                "hashtag",
                "serbisyopubliko",
                "cbb"
            ];
            List<string> sectionsToScrape = [];
            char[] separators = [',', ' '];
            var validInputSectionsToScrape = GetValidInputsForSectionsToScrape();
            var inputSectionArray = validInputSectionsToScrape.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            sectionsToScrape.AddRange(inputSectionArray);

            var filteredUrlPaths = urlPaths
                .Where(section => sectionsToScrape.Any(keyword => section.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            return filteredUrlPaths;
        }
        private static string GetValidInputsForSectionsToScrape()
        {
            List<string> keywordsToKeep = [];
            Console.WriteLine("Enter section/s to scrape (separate by a comma):");
            Console.WriteLine("topstories");
            Console.WriteLine("money");
            Console.WriteLine("sports");
            Console.WriteLine("pinoyabroad");
            Console.WriteLine("sci-tech");
            Console.WriteLine("showbiz");
            Console.WriteLine("lifestyle");
            Console.WriteLine("opinion");
            Console.WriteLine("hashtag");
            Console.WriteLine("serbisyopubliko");
            Console.WriteLine("cbb");
            Console.WriteLine("For example: topstories,money");
            string inputSectionsToScrape = Console.ReadLine();
            var validInputSectionsToScrape = ThrowIfNullOrEmptyAndReturnIfNot(inputSectionsToScrape, "Didn't provide sections to scrape");
            return validInputSectionsToScrape!;
        }
        private static string ThrowIfNullOrEmptyAndReturnIfNot(string? input, string exceptionParameter)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException(exceptionParameter);
            } else
            {
                return input!;
            }
        }
    }
}
