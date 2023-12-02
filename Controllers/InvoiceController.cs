using Microsoft.AspNetCore.Mvc;
using Invoice.Helpers;
using Invoice.Models;


namespace Invoice.Controllers
{
    [Route("v1/invoice")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {

        private readonly ILogger<InvoiceController> _logger;
        private readonly CsvFileReader _csvFileReader;
        public List<InvoiceCsvLine> invoiceCsvLines { get; set; }

        public List<int> indexes = new List<int>();
        public List<string> errorsList = new List<string>();
        public List<Dictionary<string,string>> main;
        bool exists;
        
        public InvoiceController(ILogger<InvoiceController> logger)
        {
            this._logger = logger;
            _csvFileReader = new CsvFileReader();
            invoiceCsvLines = new List<InvoiceCsvLine>();
            exists = true;
            indexes.Add(0);
            indexes.Add(2);
            errorsList.Add("Greska 1");
            errorsList.Add("Greska 2");
            main = new List<Dictionary<string,string>>();
            exists = false;

        }


        [Produces("application/json")]
        [Route("import")]
        [HttpPost]
        public async Task<IActionResult> ImportTransactionsAsync([FromForm] IFormFile formFile)
        {

            try
            {
                if (!await _csvFileReader.GetCsvReader(formFile)) { throw new Exception("File format not supported!"); }

                invoiceCsvLines = _csvFileReader.invoiceCsvLines;

                foreach (InvoiceCsvLine line in invoiceCsvLines)
                {
                    if (line.id != null)
                    {
                        Dictionary<string, string> values = getJObject(line); //JObject
                        main.Add(values); // JArray
                    }
                }
                return Ok(main);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        public Dictionary<string,string> getJObject(InvoiceCsvLine invoiceCsvLine)
        {
            Dictionary<string, string> value = new Dictionary<string, string>();

            string[] fieldsArray = invoiceCsvLine.toArray();

            for (int i = 0; i < fieldsArray.Length; i++)
            {
                value[i.ToString()] = fieldsArray[i].ToString();
            }

            if (!value["1"].Equals("!") && !value["1"].Equals("T"))
            {
                for(int i = 0; i < indexes.Count; i++)
                {
                    int invoiceNo = indexes.ElementAt(i) + 1;
                    if (invoiceNo.ToString().Equals(value["0"]))
                    {
                        value[fieldsArray.Length.ToString()] = errorsList[i];
                    }
                }
            }

            return value;
        }
    }
}

