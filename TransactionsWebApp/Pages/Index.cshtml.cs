using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TransactionsWebApp.Data;
using TransactionsWebApp.Model;

namespace TransactionsWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _db;

        public IEnumerable<Transactions> Transactions { get; set; }
        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

       
        public void OnGet()
        {
            Transactions = _db.Transactions;
        }
        public IActionResult OnPost() 
        {
            return Page();
        }
    }
}