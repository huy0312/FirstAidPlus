using FirstAidPlus.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirstAidPlus.Areas.Admin.ViewModels;

namespace FirstAidPlus.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TransactionController : Controller
    {
        private readonly AppDbContext _context;

        public TransactionController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? status, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Transactions
                .Include(t => t.User)
                .Include(t => t.Plan)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearch = searchTerm.ToLower();
                query = query.Where(t => 
                    (t.User != null && (t.User.FullName.ToLower().Contains(lowerSearch) || t.User.Email.ToLower().Contains(lowerSearch))) ||
                    (t.VnPayTxnRef != null && t.VnPayTxnRef.ToLower().Contains(lowerSearch)) ||
                    (t.MomoOrderId != null && t.MomoOrderId.ToLower().Contains(lowerSearch)) ||
                    t.Id.ToString().Contains(lowerSearch)
                );
            }

            // Filter
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));

            var transactions = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new TransactionListVM
            {
                Transactions = transactions,
                SearchTerm = searchTerm,
                StatusFilter = status,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.User)
                .Include(t => t.Plan)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }
    }
}

