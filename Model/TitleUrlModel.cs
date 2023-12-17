namespace ExtractNews.Model
{
    public class TitleUrlModel
    {
        public string Title { get; set; } = string.Empty;
        public Uri NewsUrl { get; set; }
        public Uri ImageUrl { get; set; }
        public string SubSection { get; set; } = string.Empty;
        public DateTime DateTimeUploaded { get; set; }
    }
}
