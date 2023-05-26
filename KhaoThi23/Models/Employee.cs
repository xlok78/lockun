using System.ComponentModel.DataAnnotations;

namespace KhaoThi23.Models
{
    public class Employee
    {
        [Key]
        public string EmployeeId { get; set; }
        public string EmployeeGender { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeMSSV { get; set; }
        public string CreateAt { get; set; }
        public string UpdateAt { get; set; }

        public string AccountId { get; set; }
        public virtual Account Account { get; set; }
    }
}
