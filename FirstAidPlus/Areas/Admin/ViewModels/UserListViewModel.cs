using FirstAidPlus.Models;

namespace FirstAidPlus.Areas.Admin.ViewModels
{
    public class UserListViewModel
    {
        public IEnumerable<User> Users { get; set; } = new List<User>();
        public string? SearchString { get; set; }
        public int? RoleFilter { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>();

        public CreateUserViewModel NewUser { get; set; } = new CreateUserViewModel();

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
