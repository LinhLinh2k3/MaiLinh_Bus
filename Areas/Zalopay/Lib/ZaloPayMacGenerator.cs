using NhaXeMaiLinh.Areas.Zalopay.Lib.Crypto;

namespace NhaXeMaiLinh.Areas.Zalopay.Lib
{
    public class ZaloPayMacGenerator
    {
        private static IConfigurationRoot _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

        public static string Compute(string data, string key = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                key = _config["Thanh-toan:ZaloPay:Key1"]!;
            }

            return HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key, data);
        }

        private static string CreateOrderMacData(Dictionary<string, string> order)
        {
            return order["appid"] + "|" + order["app_trans_id"] + "|" + order["app_user"] + "|" + order["amount"]
              + "|" + order["apptime"] + "|" + order["embed_data"] + "|" + order["item"];
        }

        public static string CreateOrder(Dictionary<string, string> order)
        {
            return Compute(CreateOrderMacData(order));
        }

        public static string Refund(Dictionary<string, string> data)
        {
            return Compute(data["app_id"] + "|" + data["zp_trans_id"] + "|" + data["amount"] + "|" + data["description"] + "|" + data["timestamp"]);
        }

        public static string GetOrderStatus(Dictionary<string, string> data)
        {
            return Compute(data["app_id"] + "|" + data["app_trans_id"] + "|" + _config["Thanh-toan:ZaloPay:Key1"]);
        }

        public static string GetRefundStatus(Dictionary<string, string> data)
        {
            return Compute(data["app_id"] + "|" + data["m_refund_id"] + "|" + data["timestamp"]);
        }

        public static string Redirect(Dictionary<string, object> data)
        {
            return Compute(data["app_id"] + "|" + data["app_trans_id"] + "|" + data["pmcid"] + "|" + data["bankcode"]
                + "|" + data["amount"] + "|" + data["discount_amount"] + "|" + data["status"]);
        }
    }
}