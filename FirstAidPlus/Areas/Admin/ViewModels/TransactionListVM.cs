using FirstAidPlus.Models;
using System.Collections.Generic;

namespace FirstAidPlus.Areas.Admin.ViewModels
{
    public class TransactionListVM
    {
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public string? SearchTerm { get; set; }
        public string? StatusFilter { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
    }
}
