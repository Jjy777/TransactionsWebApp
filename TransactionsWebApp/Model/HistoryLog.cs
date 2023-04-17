using System.ComponentModel.DataAnnotations;

namespace TransactionsWebApp.Model
{
    public class HistoryLog
    {
        [Key]
        public int id { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
