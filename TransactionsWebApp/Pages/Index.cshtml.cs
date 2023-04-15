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
        private readonly int _maxFileSize = 220000 ;
            /*1 * 1024 * 1024;*/

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
        public IActionResult OnPost(IFormFile file) 
        {
            if (file != null)
            {
                string strFileName = Path.GetFileName(file.FileName);
                string strExtension = Path.GetExtension(strFileName);
                if (file.Length > _maxFileSize)
                {
                    return BadRequest("Maximum allowed file size is 1 MB.");
                }
                else {
                    if (strExtension == ".xml" || strExtension == ".csv")
                    {
                        return new OkResult();

                    }
                    else
                    {
                        return BadRequest("Only CSV file and XML file can be accepted.");
                       
                    }

                }
            }
            return Page();
        }
        
    }
}