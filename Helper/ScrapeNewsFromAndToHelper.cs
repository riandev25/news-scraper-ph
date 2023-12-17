using ExtractNews.Model;
using PuppeteerSharp;

namespace ExtractNews.Helper
{
    public static class ScrapeNewsFromAndToHelper
    {
        public static async Task<TitleUrlModel[]> ScrapeNewsFromAndTo(IPage page, string path, string toMonth, int toYear, string fromMonth, int fromYear)
        {
            int actualMonthStart = GetMonthValue(fromMonth);
            int actualYearStart = fromYear;
            int currentYear = DateTime.Now.Year;

            // automate to pick specific month and year to start scraping news data\
            //string simulatedPickStartYear;  
            string simulatedPickStartMonth;
            await page.GoToAsync($"https://www.gmanetwork.com/news/archives/{path}/");

            if (fromYear == currentYear)
            {
                await page.ClickAsync($"#accordion > div[aria-hidden=\'false\'] ul li[data-month=\'{actualMonthStart}\']");
            }
            else
            {
                await page.ClickAsync($"#accordion h5#year-{actualYearStart}");
                int accordionPanelNumber = GetAccordionPanelNumber(fromYear);
                await page.WaitForTimeoutAsync(3000);
                await page.ClickAsync($"#ui-accordion-accordion-panel-{accordionPanelNumber} ul li[data-month=\'{actualMonthStart}\']");
            }
            
            
            //await Task.Delay(3000);

            // automate to pick specific month and year to end scraping news data
            int actualMonthEnd = GetActualMonthToEndScrape(GetMonthValue(toMonth));
            int actualYearEnd = GetActualYearToEndScrape(GetMonthValue(toMonth), toYear);
            bool isMonthAndYearEndVisible;
            do
            {
                await page.EvaluateExpressionAsync("window.scrollBy(0, window.innerHeight)");
                string simulatedIsMonthAndYearEndVisible = $@"document.querySelector('#grid_thumbnail_stories h3[data-month=""{actualMonthEnd}""][data-year=""{actualYearEnd}""]') !== null";
                isMonthAndYearEndVisible = await page.EvaluateExpressionAsync<bool>(simulatedIsMonthAndYearEndVisible);
            } while (!isMonthAndYearEndVisible);

            //DateTimeUploaded: new Date(new Date(a.querySelector('.archive_date_time').textContent.trim()).toLocaleString(""en - US"", { timeZone: ""UTC"" })).toUTCString(),

            //string simulatedScrapedDataToArray = @"Array.from(document.getElementById('grid_thumbnail_stories').querySelectorAll('a')).map(a => ({
            //                    Title: Array.from(a.querySelector('.story_title').childNodes)
            //                        .filter(node => node.nodeType === Node.TEXT_NODE)
            //                        .map(node => node.textContent.trim())
            //                        .join(' ')
            //                        .replace(/,*$/, ''),
            //                    NewsUrl: a.href,
            //                    ImageUrl: a.querySelector('img').getAttribute('data-original'),
            //                    SubSection: a.querySelector('.subsection').textContent.trim(),
            //                    DateTimeUploaded: new Date(new Date(a.querySelector('.archive_date_time').textContent.trim()).toLocaleString(""en-US"", { timeZone: ""Asia/Manila"" })).toUTCString(),
            //                }));";
            string simulatedScrapedDataToArray = @"Array.from(document.getElementById('grid_thumbnail_stories').querySelectorAll('a')).map(a => {
    const titleNode = a.querySelector('.story_title');
    const subsectionNode = a.querySelector('.subsection');
    const dateTimeNode = a.querySelector('.archive_date_time');
    const imgNode = a.querySelector('img');

    return {
        Title: titleNode && titleNode.childNodes
            ? Array.from(titleNode.childNodes)
                .filter(node => node.nodeType === Node.TEXT_NODE)
                .map(node => node.textContent ? node.textContent.trim() : null)
                .join(' ')
                .replace(/,*$/, '')
            : null,
        NewsUrl: a.href,
        ImageUrl: imgNode ? imgNode.getAttribute('data-original') : null,
        SubSection: subsectionNode && subsectionNode.textContent
            ? subsectionNode.textContent.trim()
            : null,
        DateTimeUploaded: dateTimeNode && dateTimeNode.textContent
            ? new Date(new Date(dateTimeNode.textContent.trim()).toLocaleString('en-US', { timeZone: 'Asia/Manila' })).toUTCString()
            : null,
    };
})";
            var scrapedData = await page.EvaluateExpressionAsync<TitleUrlModel[]>(simulatedScrapedDataToArray);
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



//using ExtractNews.Model;
//using PuppeteerSharp;


//namespace ExtractNews.Helper
//{
//    public static class ScrapeNewsFromAndToHelper
//    {
//        public static MonthYearModel ScrapeNewsFromAndTo(string fromMonth, int fromYear, string toMonth, int toYear)
//        {
//            //List<TitleUrlModel> scrapedData = [];
//            int actualMonthStart = GetMonthValue(fromMonth);
//            int actualYearStart = fromYear;
//            int currentYear = DateTime.Now.Year;

//            // automate to pick specific month and year to start scraping news data
//            //string simulatedPickStartMonthAndYear;
//            //if (fromYear == currentYear)
//            //{
//            //    simulatedPickStartMonthAndYear = $@"const li = document.querySelector(""#accordion > div[aria-hidden=\'false\'] ul li[data-month=\'{actualMonthStart}\']"");
//            //        li.click();";
//            //}
//            //else
//            //{
//            //    simulatedPickStartMonthAndYear = $@"const simulatedPickStartYear = document.querySelector(""#accordion h5#year-{actualYearStart}"");
//            //        simulatedPickstartYear.click();
//            //        const li = document.querySelector(""#accordion > div[aria-hidden=\'false\'] ul li[data-month=\'{actualMonthStart}\']"");
//            //        li.click();";
//            //}
//            //await page.EvaluateExpressionAsync(simulatedPickStartMonthAndYear);
//            //await Task.Delay(3000);

//            // automate to pick specific month and year to end scraping news data
//            int actualMonthEnd = GetActualMonthToEndScrape(GetMonthValue(toMonth));
//            int actualYearEnd = GetActualYearToEndScrape(GetMonthValue(toMonth), toYear);
//            //bool isMonthAndYearEndVisible;
//            //do
//            //{
//            //    await page.EvaluateExpressionAsync("window.scrollBy(0, window.innerHeight)");
//            //    string simulatedIsMonthAndYearEndVisible = $@"document.querySelector('#grid_thumbnail_stories h3[data-month=""{actualMonthEnd}""][data-year=""{actualYearEnd}""]') !== null";
//            //    isMonthAndYearEndVisible = await page.EvaluateExpressionAsync<bool>(simulatedIsMonthAndYearEndVisible);
//            //} while (!isMonthAndYearEndVisible);

//            //string simulatedScrapedDataToArray = @"Array.from(document.getElementById('grid_thumbnail_stories').querySelectorAll('a')).map(a => ({
//            //                    Title: a.querySelector('.story_title').textContent.trim(),
//            //                    Url: a.href,
//            //                    Datetime: a.querySelector('.archive_date_time').textContent.trim()
//            //                }));";
//            //var scrapedData = await page.EvaluateExpressionAsync<TitleUrlModel[]>(simulatedScrapedDataToArray);
//            //return scrapedData;
//            return new MonthYearModel
//            {
//                actualMonthStart = actualMonthStart,
//                actualYearStart = actualYearStart,
//                actualMonthEnd = actualMonthEnd,
//                actualYearEnd = actualYearEnd,
//            };
//        }
//        private static int GetMonthValue(string monthName)
//        {
//            if (string.IsNullOrWhiteSpace(monthName))
//                throw new ArgumentException("Month name cannot be null or empty.", nameof(monthName));

//            string[] monthNames = { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };

//            int monthIndex = Array.IndexOf(monthNames, monthName.ToLower());

//            if (monthIndex == -1)
//                throw new ArgumentException("Invalid month name.", nameof(monthName));

//            return monthIndex;
//        }
//        private static int GetActualMonthToEndScrape(int convertedToMonth)
//        {
//            // when the month that scraping ends is for example is January which has a month value of 0,
//            // for example:
//            // <h5>December 2023</h5> --> data-month = 11
//            //      ...data
//            // <h5>March 2023</h5> --> data-month = 2
//            //      ...data
//            // <h5>January 2023</h5> --> data-month = 0
//            //      ...data
//            // <h5>January 2022</h5> --> data-month = 11
//            // because of this structure to scrape data from December to January, it will need to start and target h5 dec 2023 and end scrape at h5 jan 2022
//            int actualToMonthValue = convertedToMonth == 0 ? 11 : convertedToMonth - 1;
//            return actualToMonthValue;
//        }
//        private static int GetActualYearToEndScrape(int toMonth, int toYear)
//        {
//            int actualYearToEnd = toMonth == 0 ? toYear - 1 : toYear;
//            return actualYearToEnd;
//        }
//    }
//}

