using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaaStarRemote.Models
{
    public class Tasks
    {

        [Key]
        public string id { get; set; }

        [ForeignKey("Users")]
        public string user { get; set; }

        [Required]
        public string task { get; set; }

        [Required]
        public int interval { get; set; }

        [Required]
        public DateTime time { get; set; }

        [Required]
        public string uuid { get; set; }
    }
}
