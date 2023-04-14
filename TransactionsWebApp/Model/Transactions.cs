using System.ComponentModel.DataAnnotations;

namespace TransactionsWebApp.Model
{
    public class Transactions
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string TransactionID { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
