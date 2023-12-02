namespace Invoice.Models
{
    public class InvoiceCsvLine
    {

        public string id { get; set; }
        public string beneficiaryname { get; set; }
        public string date { get; set; }
        public string direction { get; set; }
        public string error { get; set; }

        internal string[] toArray()
        {
            string[] fieldsArray = new string[4];
            fieldsArray[0] = this.id;
            fieldsArray[1] = this.beneficiaryname;
            fieldsArray[2] = this.date;
            fieldsArray[3] = this.direction;
            return fieldsArray;
        }
    }
}

