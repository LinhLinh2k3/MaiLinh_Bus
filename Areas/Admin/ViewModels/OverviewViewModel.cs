namespace NhaXeMaiLinh.Areas.Admin.ViewModels
{
    public class OverviewViewModel
    {
        // Khuyến mãi
        public int TotalPromotions { get; set; }
        public int ActivePromotions { get; set; }

        // Nhân viên
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }

        // Lịch trình
        public int TotalSchedules { get; set; }
        public int SchedulesToday { get; set; }

        // Xe
        public int TotalVehicles { get; set; }
        public int ActiveVehicles { get; set; }

        // Tài xế
        public int TotalDrivers { get; set; }
        public int AvailableDrivers { get; set; }
    }
}
