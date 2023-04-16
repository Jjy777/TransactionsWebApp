using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TransactionsWebApp.Data;
using TransactionsWebApp.Model;
using System.Transactions;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace TransactionsWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _db;
        private IHostingEnvironment _environment;

        private readonly int _maxFileSize = 220000 ;
            /*1 * 1024 * 1024;*/

        public IEnumerable<Transactions> Transactions { get; set; }
        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext db, IHostingEnvironment environment)
        {
            _logger = logger;
            _db = db;
            _environment = environment;
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

                string[] statusXML = {"Approved", "Rejected", "Done" };
                string[] statusCSV = { "Approved", "Failed", "Finished"};

                if (file.Length > _maxFileSize)
                {
                    return BadRequest("Maximum allowed file size is 1 MB.");
                }
                else {
                    if (strExtension != "" && file.ContentType != null)
                    {
                        if (strExtension == ".xml" || strExtension == ".csv")
                        {
                            try{
                                List<Transactions> tList = new List<Transactions>();
                                var stream = file.OpenReadStream();
                                var Content = "";
                                using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                                {
                                    Content = streamReader.ReadToEnd();
                                }
                                if (strExtension == ".xml"){

                                    //Load the XML in XmlDocument.                                 
                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(string.Concat(Content));

                                    //Loop through the selected Nodes.
                                    foreach (XmlNode node in doc.SelectNodes("/Transactions/Transaction"))
                                    {
                                        //Fetch the Node values and assign it to object.
                                        if (Array.Exists(statusXML, element => element == node["Status"].InnerText))
                                        {
                                            string statusReplaced = "";
                                            if (node["Status"].InnerText == "Approved")
                                            {
                                                statusReplaced = "A";
                                            }
                                            else if (node["Status"].InnerText == "Rejected")
                                            {
                                                statusReplaced = "R";
                                            }
                                            else if (node["Status"].InnerText == "Done")
                                            {
                                                statusReplaced = "D";
                                            }

                                            if (node.Attributes["id"].Value != null && node.Attributes["id"].Value != "")
                                            {
                                                tList.Add(new Transactions
                                                {
                                                    TransactionID = node.Attributes["id"].Value,
                                                    TransactionDate = DateTime.ParseExact(node["TransactionDate"].InnerText, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture),
                                                    Status = statusReplaced,
                                                    Amount = decimal.Parse(node["PaymentDetails"]["Amount"].InnerText),
                                                    CurrencyCode = node["PaymentDetails"]["CurrencyCode"].InnerText,
                                                    CreatedDate = DateTime.Now
                                                });
                                            }
                                            else
                                            {
                                                throw new Exception();
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception();
                                        }
                                    }                              
                                }
                                else{

                                    //Loop through the rows.
                                    foreach (string row in Content.Split('\n'))
                                    {
                                        Transactions t = new Transactions();
                                        string statusReplaced ="" ;

                                        if (!string.IsNullOrEmpty(row))
                                        {
                                            //Execute a loop over the columns.
                                            int i = 0;
                                            TextFieldParser parser = new TextFieldParser(new StringReader(row));
                                            parser.HasFieldsEnclosedInQuotes = true;
                                            parser.SetDelimiters(",");
                                            string[] fields;

                                            while (!parser.EndOfData)
                                            {
                                                fields = parser.ReadFields();
                                                foreach (string field in fields)
                                                {
                                                    if (i == 0)
                                                    {
                                                        t.TransactionID = field;
                                                    }
                                                    else if (i == 1)
                                                    {
                                                        t.Amount = decimal.Parse(field);
                                                    }
                                                    else if (i == 2)
                                                    {
                                                        t.CurrencyCode = field;
                                                    }
                                                    else if (i == 3)
                                                    {
                                                        t.TransactionDate = DateTime.ParseExact(field, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                                    }
                                                    else if (i == 4)
                                                    {

                                                        if (field == "Approved")
                                                        {
                                                            statusReplaced = "A";
                                                        }
                                                        else if (field == "Failed")
                                                        {
                                                            statusReplaced = "R";
                                                        }
                                                        else if (field == "Finished")
                                                        {
                                                            statusReplaced = "D";
                                                        }
                                                        t.Status = statusReplaced;
                                                    }
                                                    i++;
                                                }
                                            }

                                            parser.Close();                                           
                                            t.CreatedDate = DateTime.Now;
                                            tList.Add(t);
                                        }
                                        else
                                        {
                                            throw new Exception();
                                        }

                                    }
                              
                                }
                                if (tList.Count>0) {
                                    foreach (Transactions t in tList) {
                                        _db.Transactions.Add(t);
                                        _db.SaveChangesAsync();
                                    }

                                }

                            }catch (Exception e)
                            {
                                return BadRequest("Record Invalid");
                            }
                            return new OkResult();

                        }
                        else
                        {
                            return BadRequest("Only CSV file and XML file can be accepted.");

                        }
                    }
                    else
                    {
                        return BadRequest("Unknown format");

                    }
                    

                }
            }
            return Page();
        }
       

    }
}