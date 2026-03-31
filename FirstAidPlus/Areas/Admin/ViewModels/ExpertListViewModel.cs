using FirstAidPlus.Models;

namespace FirstAidPlus.Areas.Admin.ViewModels
{
    public class ExpertListViewModel
    {
        public List<User> Experts { get; set; } = new List<User>();
        public string? SearchString { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
