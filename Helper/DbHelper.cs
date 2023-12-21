using ExtractNews.EfCore.Context;
using ExtractNews.EfCore.Models;
using ExtractNews.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractNews.Helper
{
    public class DbHelper(NewsDbContext context)
    {
        private readonly NewsDbContext _context = context;

        public void SaveOrder(List<RawNewsModel> rawNewsList)
        {
            List<RawNewsModel> sortedRawNewListByDatetime = [.. rawNewsList.OrderByDescending(news => news.DateTimeUploaded)];

            foreach (var rawNewsEach in sortedRawNewListByDatetime)
            {
                RawNews rawNews = new();
                RawNews dbTable = rawNews;
                Console.WriteLine($"Title: {rawNewsEach.Title}, NewsUrl: {rawNewsEach.NewsUrl}, ImageUrl: {rawNewsEach.ImageUrl}, SubSection: {rawNewsEach.SubSection}, DatetimeUploaded: {rawNewsEach.DateTimeUploaded}");
                dbTable.NewsUrl = new Uri(rawNewsEach.NewsUrl.ToString());
                dbTable.ImageUrl = new Uri(rawNewsEach.ImageUrl.ToString());
                dbTable.Title = rawNewsEach.Title;
                dbTable.Section = rawNewsEach.Section;
                dbTable.SubSection = rawNewsEach.SubSection;
                dbTable.DateTimeUploaded = rawNewsEach.DateTimeUploaded;
                _context.RawNews.Add(dbTable);
            };
            _context.SaveChanges();
        }
    }
}
