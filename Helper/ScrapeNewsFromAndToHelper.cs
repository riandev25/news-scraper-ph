using ExtractNews.Model;
using PuppeteerSharp;

namespace ExtractNews.Helper
{
    public static class ScrapeNewsFromAndToHelper
    {
        public static async Task<RawNewsModel[]> ScrapeNewsFromAndTo(IPage page, string path, string toMonth, int toYear, string fromMonth, int fromYear)
        {
            int actualMonthStart = GetMonthValue(fromMonth);
            int actualYearStart = fromYear;
            int currentYear = DateTime.Now.Year;

            var navigationOptions = new NavigationOptions { Timeout = 60000 };
            await page.GoToAsync($"https://www.gmanetwork.com/news/archives/{path}/", navigationOptions);

            // base on the website's behaviour, the list of buttons for months are automatically opened for the current year
            if (fromYear == currentYear)
            {
                await page.ClickAsync($"#accordion > div[aria-hidden=\'false\'] ul li[data-month=\'{actualMonthStart}\']");
            }
            else
            {

                await page.ClickAsync($"#accordion h5#year-{actualYearStart}");
                await page.WaitForSelectorAsync($"#accordion h5#year-{actualYearStart}");
                int accordionPanelNumber = GetAccordionPanelNumber(fromYear);
                await page.WaitForTimeoutAsync(3000);
                await page.ClickAsync($"#ui-accordion-accordion-panel-{accordionPanelNumber} ul li[data-month=\'{actualMonthStart}\']");
            }

            // wait for loading the first batch of news before proceeding, it will load even if there's no news available
            bool isLoading;
            do
            {
                isLoading = await page.EvaluateExpressionAsync<bool>(@"document.querySelector('#grid_thumbnail_container .grid_thumbnail_loading').style.display === ""none"" ? true : false");
            } while (!isLoading);

            int actualMonthEnd = GetActualMonthToEndScrape(GetMonthValue(toMonth));
            int actualYearEnd = GetActualYearToEndScrape(GetMonthValue(toMonth), toYear);
            bool isMonthAndYearEndVisible;

            // continously scroll until getting the desired data
            do
            {
                await page.EvaluateExpressionAsync("window.scrollBy(0, window.innerHeight)");
                await page.EvaluateExpressionAsync("window.scrollBy(0, -0.001)");
                string simulatedIsMonthAndYearEndVisible = $@"document.querySelector('#grid_thumbnail_stories h3[data-month=""{actualMonthEnd}""][data-year=""{actualYearEnd}""]') !== null";

                isMonthAndYearEndVisible = await page.EvaluateExpressionAsync<bool>(simulatedIsMonthAndYearEndVisible);
            } while (!isMonthAndYearEndVisible);

            string simulatedScrapedDataToArray = $@"Array.from(document.getElementById('grid_thumbnail_stories').querySelectorAll('a')).map(a => {{
                const titleNode = a.querySelector('.story_title');
                const subsectionNode = a.querySelector('.subsection');
                const dateTimeNode = a.querySelector('.archive_date_time');
                const imgNode = a.querySelector('img');

                return {{
                    Title: titleNode && titleNode.childNodes
                        ? Array.from(titleNode.childNodes)
                            .filter(node => node.nodeType === Node.TEXT_NODE)
                            .map(node => node.textContent ? node.textContent.trim() : null)
                            .join(' ')
                            .replace(/,*$/, '')
                        : null,
                    NewsUrl: a.href,
                    ImageUrl: imgNode ? imgNode.getAttribute('data-original') : null,
                    Section: '{path}',
                    SubSection: subsectionNode && subsectionNode.textContent
                        ? subsectionNode.textContent.trim()
                        : null,
                    DateTimeUploaded: dateTimeNode && dateTimeNode.textContent
                        ? new Date(new Date(dateTimeNode.textContent.trim()).toLocaleString('en-US', {{ timeZone: 'Asia/Manila' }})).toUTCString()
                        : null,
                    }};
                }})";
            var scrapedData = await page.EvaluateExpressionAsync<RawNewsModel[]>(simulatedScrapedDataToArray);
            return scrapedData;
        }
        private static int GetMonthValue(string monthName)
        {
            if (string.IsNullOrWhiteSpace(monthName))
                throw new ArgumentException("Month name cannot be null or empty.", nameof(monthName));

            string[] monthNames = ["january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december"];

            int monthIndex = Array.IndexOf(monthNames, monthName.ToLower());

            if (monthIndex == -1)
                throw new ArgumentException("Invalid month name.", nameof(monthName));

            return monthIndex;
        }
        private static int GetActualMonthToEndScrape(int convertedToMonth)
        {
            // when the month that scraping ends is for example is January which has a month value of 0,
            // for example:
            // <h5>December 2023</h5> --> data-month = 11
            //      ...data
            // <h5>March 2023</h5> --> data-month = 2
            //      ...data
            // <h5>January 2023</h5> --> data-month = 0
            //      ...data
            // <h5>January 2022</h5> --> data-month = 11
            // because of this structure to scrape data from December to January, it will need to start and target h5 dec 2023 and end scrape at h5 jan 2022
            int actualToMonthValue = convertedToMonth == 0 ? 11 : convertedToMonth - 1;
            return actualToMonthValue;
        }
        private static int GetActualYearToEndScrape(int toMonth, int toYear)
        {
            int actualYearToEnd = toMonth == 0 ? toYear - 1 : toYear;
            return actualYearToEnd;
        }
        private static int GetAccordionPanelNumber(int fromYear)
        {
            int currentYear = DateTime.Now.Year;
            int accordionPanelNumber = currentYear - fromYear;
            return accordionPanelNumber;
        }
    }
}

