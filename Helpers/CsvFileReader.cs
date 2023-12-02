using CsvHelper;
using CsvHelper.Configuration;
using System.Text.RegularExpressions;
using Invoice.Models;

namespace Invoice.Helpers
{
    public class CsvFileReader
    {

        public List<InvoiceCsvLine>  invoiceCsvLines { get; set; }

        public CsvFileReader()
        {
            invoiceCsvLines = new List<InvoiceCsvLine>();
        }


        /// <summary>
        /// Gets a StreamReader for the provided CSV file.
        /// </summary>
        /// <param name="formFile">The CSV file uploaded.</param>
        /// <param name="type">An integer representing the type of CSV data.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean value
        /// indicating whether the CSV reader was obtained successfully for the provided type.
        /// </returns>
        public async Task<Boolean> GetCsvReader(IFormFile formFile)
        {

            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream);
                stream.Position = 0;
                byte[] bytes = stream.ToArray();

                try
                {
                    using (var fileStream = new FileStream(formFile.FileName, FileMode.Create, FileAccess.Write))
                    {
                        fileStream.Write(bytes, 0, bytes.Length);
                        fileStream.Close();
                        using (var reader = new StreamReader(fileStream.Name, System.Text.Encoding.UTF8))
                        {
                            var config = new CsvConfiguration(System.Globalization.CultureInfo.CreateSpecificCulture("enUS"))
                            {
                                Delimiter = ";",
                                HasHeaderRecord = false,
                                TrimOptions = TrimOptions.Trim,
                                MissingFieldFound = null,
                                PrepareHeaderForMatch = args => Regex.Replace(args.Header, "-", "").ToLower()
                            };

                            var csv = new CsvReader(reader, config);
                            invoiceCsvLines = csv.GetRecords<InvoiceCsvLine>().ToList();

                        
                        }

                    }
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

        }
    }

}