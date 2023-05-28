using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KhaoThi23.Models
{
    public class News
    {
        [Key]
        public int NewsId { get; set; }

        public string EmployeeId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string? Content1 { get; set; }
        public string Image1 { get; set; }
        public string? ImageDesc1 { get; set; }

        public string? Content2 { get; set; }
        public string? Image2 { get; set; }
        public string? ImageDesc2 { get; set; }

        public string? Content3 { get; set; }
        public string? Image3 { get; set; }
        public string? ImageDesc3 { get; set; }

        public string? Content4 { get; set; }
        public string? Image4 { get; set; }
        public string? ImageDesc4 { get; set; }

        public string? Content5 { get; set; }
        public string? Image5 { get; set; }
        public string? ImageDesc5 { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }


        public Employee Employee { get; set; }
    }
}
