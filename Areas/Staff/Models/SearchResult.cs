using NhaXeMaiLinh.Models.Data;

namespace NhaXeMaiLinh.Areas.Staff.Models
{
    public class SearchResult
    {
        public List<KhachHang> KhachHangs { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        // Calculate the total number of pages
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }
}