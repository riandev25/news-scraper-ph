namespace news_scraper_ph.Helper
{
    public static class ScriptInputsHelper
    {
        public static (string, int, string, int) GetValidInputsForDateScrapeRange()
        {
            // start month input handler
            Console.WriteLine("Month start (ex. november):");
            string? inputMonthStart = Console.ReadLine();
            var validInputMonthStart = ThrowIfNullOrEmptyAndReturnIfNot(inputMonthStart, "Didn't provide month start to scrape");

            // start year input handler
            Console.WriteLine("Year start (ex. 2023):");
            string inputYearStart = Console.ReadLine();
            var validInputYearStart = ThrowIfNullOrEmptyAndReturnIfNot(int.Parse(inputYearStart), "Year provided is out of range");

            // end month input handler
            Console.WriteLine("Month end (ex. december):");
            string inputMonthEnd = Console.ReadLine();
            var validInputMonthEnd = ThrowIfNullOrEmptyAndReturnIfNot(inputMonthEnd, "Didn't provide month end to scrape");

            // end date input handler
            Console.WriteLine("Year end (ex. 2023):");
            string inputYearEnd = Console.ReadLine();
            var validInputYearEnd = ThrowIfNullOrEmptyAndReturnIfNot(int.Parse(inputYearEnd), "Year provided is out of range");
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
                "scitech",
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
            Console.WriteLine("scitech");
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
        private static int ThrowIfNullOrEmptyAndReturnIfNot(int input, string exceptionParameter)
        {
            var currentYear = DateTime.Now.Year;

            if (input < 2008 && input > currentYear)
            {
                throw new ArgumentException(exceptionParameter);
            }
            else
            {
                return input!;
            }
        }
    }
}
