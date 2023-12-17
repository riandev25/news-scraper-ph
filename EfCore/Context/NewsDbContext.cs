using ExtractNews.EfCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ExtractNews.EfCore.Context
{
    public class NewsDbContext : DbContext
    {
        public NewsDbContext()
        {
        }

        public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options) { }
        public DbSet<RawNews> RawNews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            options.UseNpgsql(config["ConnectionStrings:PostgreSqlServer"]);
        }
    }
}
