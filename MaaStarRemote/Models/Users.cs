using System.ComponentModel.DataAnnotations;

namespace MaaStarRemote.Models
{
    public class Users
    {
        [Required]
        public string user { get; set; }

        [Required]
        public string device { get; set; }


    }
}
