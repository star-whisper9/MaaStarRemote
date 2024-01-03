using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaaStarRemote.Models
{
    public class Reports
    {
        [Required]
        [ForeignKey("Users")]
        public string user { get; set; }

        [Required]
        [ForeignKey("Users")]
        public string device { get; set; }

        [Key]
        public string task { get; set; }

        [Required]
        public string status { get; set; } 

        public string payload { get; set; }

        [Required]
        public DateTime time { get; set; }

        [Required]
        public string info { get; set; }
    }
}
