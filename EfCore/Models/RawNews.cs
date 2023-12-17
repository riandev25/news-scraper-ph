using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractNews.EfCore.Models
{
    [Table("raw_news")]
    public class RawNews
    {
        [Key, Required]
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        [Url]
        public Uri NewsUrl { get; set; }
        [Url]
        public Uri ImageUrl { get; set; }
        public string SubSection { get; set; } = string.Empty;
        public DateTime DateTimeUploaded { get; set; }
    }
}
