# Gma News Scraper

This is news scraper tool for [Gma News Website](https://www.gmanetwork.com/news/)

## Prerequisites

This solution is built on the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and runs on .NET CLI, you need to install that before it will work for you.

## How does this work

If you'll visit gma news website, find the view more button and it will redirect to [news archives](https://www.gmanetwork.com/news/archives/just_in/). You'll see the sections of news archives: News, Money, Sports, Pinoy Abroad, Sci Tech, Showbiz, Lifestyle, Opinion, Hashtag, Serbisyo Publiko and Community Bulletin Board. This tool can scrape all the data from these sections given the user parameters to scrape.

This tool is still a work in progress. That's why in this version, it's recommended to scrape maximum of **2 months** to scrape efficiently due to the website's pagination and lazy loading behavior. Improving web scraping techniques and algorithms will be the main task for the next update to elevate the maximum months to scrape while improving the scraping time and its efficiency.

## How to run the tool

First, make sure to create an appsettings.json file in the base directory:

``` bash
{
  "ConnectionStrings": {
    "PostgreSqlServer": "Server=localhost;Database=db_name;Port=1234;User Id=id;Password=password;"
  },
}
```
This tool uses PostgreSQL Database as default and all scraped data will go to one single table "raw_news". Add different nuget package to use different database and adjust the code to your needs.

Run the project

``` bash
dotnet run --project
```

or

When using visual studio, just run the project using the GUI.

## Resources

- [Puppeteer Sharp](https://www.puppeteersharp.com/)




