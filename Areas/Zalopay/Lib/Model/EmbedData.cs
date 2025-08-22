namespace NhaXeMaiLinh.Areas.Zalopay.Lib.Model
{
    public class EmbedData
    {
        public string[] preferred_payment_method { get; set; }
        public string redirecturl { get; set; }
        public string columninfo { get; set; } // JSON String
        public string promotioninfo { get; set; } // JSON String
        public string zlppaymentid { get; set; }

        public EmbedData(string[] paymentMethod, string redirectUrl, string columnInfo,string promotionInfo, string paymentID)
        {
            preferred_payment_method = paymentMethod;
            redirecturl = redirectUrl;
            columninfo = columnInfo;
            promotioninfo = promotionInfo;
            zlppaymentid = paymentID;
        }
    }
}