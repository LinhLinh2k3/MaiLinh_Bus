using NhaXeMaiLinh.Areas.Zalopay.Lib.Crypto;

namespace NhaXeMaiLinh.Areas.Zalopay.Lib.Model
{
    public class RefundData
    {
        private static IConfigurationRoot _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

        public int app_id { get; set; }
        public string zp_trans_id { get; set; }
        public long amount { get; set; }
        public string description { get; set; }
        public long timestamp { get; set; }
        public string m_refund_id { get; set; }
        public string mac { get; set; }

        public RefundData(long amount, string zptransid, string? description)
        {
            app_id = Convert.ToInt32(_config["Thanh-toan:ZaloPay:AppID"]);
            zp_trans_id = zptransid;
            this.amount = amount;
            this.description = description ?? "";
            m_refund_id = ZaloPayHelper.GenRefundID();
            timestamp = Utils.GetTimeStamp();
            mac = ComputeMac();
        }

        public string GetMacData()
        {
            return app_id + "|" + zp_trans_id + "|" + amount + "|" + description + "|" + timestamp;
        }

        public string ComputeMac()
        {
            return HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, _config["Key1"]!, GetMacData());
        }
    }
}