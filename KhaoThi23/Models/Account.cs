using System.ComponentModel.DataAnnotations;

namespace KhaoThi23.Models
{
    public class Account
    {
        [Key]
        public string AccountId { get; set; }
        public string AccountEmail { get; set; }
        public string AccountPassword { get; set; }
        public string AccountRole { get; set; }
        public string CreateAt { get; set; }
        public string UpdateAt { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
