namespace ExtractNews.Model
{
    public class RawNewsModel
    {
        public required string Title { get; set; }
        public required Uri NewsUrl { get; set; }
        public required Uri ImageUrl { get; set; }
        public required string Section { get; set; }
        public required string SubSection { get; set; }
        public DateTime DateTimeUploaded { get; set; }
    }
}
