namespace KhaoThi23.Models
{
    public class PhucKhao
    {
        public int PhucKhaoId { get; set; }
        public string EmployeeId { get; set; }
        public string MaLop { get; set; }
        public string HocKy { get; set; }
        public string NamHoc { get; set; }
        public string MaHocPhan { get; set; }
        public string TenHocPhan { get; set; }
        public string NgayGioThi { get; set; }
        public string PhongThi { get; set; }
        public string LanThi { get; set; }
        public string LyDo { get; set; }
        public string Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; } 
        public Employee Employee { get; set; }
    }
}
