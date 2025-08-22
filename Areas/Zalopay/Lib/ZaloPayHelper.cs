using NhaXeMaiLinh.Areas.Zalopay.Lib.Crypto;
using NhaXeMaiLinh.Areas.Zalopay.Lib.Model;
using NhaXeMaiLinh.Areas.Zalopay.Lib.Extension;

namespace NhaXeMaiLinh.Areas.Zalopay.Lib
{
    public class ZaloPayHelper
    {
        private static IConfigurationRoot _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

        public static bool VerifyCallback(string data, string requestMac)
        {
            try
            {
                string mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, _config["Thanh-toan:ZaloPay:Key2"]!, data);

                return requestMac.Equals(mac);
            }
            catch
            {
                return false;
            }
        } 

        public static bool VerifyRedirect(Dictionary<string, object> data)
        {
            try
            {
                string reqChecksum = data["checksum"].ToString()!;
                string checksum = ZaloPayMacGenerator.Redirect(data);

                return reqChecksum!.Equals(checksum);
            }
            catch
            {
                return false;
            }
        }

        public static string GenTransID()
        {
            return Utils.GetFormattedDate() + "_" + Utils.GetTimeStamp();
        }

        public static string GenRefundID()
        {
            return Utils.GetFormattedDate() + "_" + _config["Thanh-toan:ZaloPay:AppID"] + "_" + Utils.GetTimeStamp();
        }

        public static Task<Dictionary<string, object>> CreateOrder(Dictionary<string, string> orderData)
        {
            return HttpHelper.PostFormAsync(_config["Thanh-toan:ZaloPay:Endpoint:CreateOrder"], orderData);
        }

        public static Task<Dictionary<string, object>> CreateOrder(OrderData orderData)
        {
            return CreateOrder(orderData.AsParams());
        }

        public static Task<Dictionary<string, object>> GetOrderStatus(string apptransid)
        {
            var data = new Dictionary<string, string>();
            data.Add("app_id", _config["Thanh-toan:ZaloPay:AppID"]!);
            data.Add("app_trans_id", apptransid);
            data.Add("mac", ZaloPayMacGenerator.GetOrderStatus(data));

            return HttpHelper.PostFormAsync(_config["Thanh-toan:ZaloPay:Endpoint:GetOrderStatus"], data);
        }

        public static Task<Dictionary<string, object>> Refund(Dictionary<string, string> refundData)
        {
            return HttpHelper.PostFormAsync(_config["Thanh-toan:ZaloPay:Endpoint:Refund"], refundData);
        }

        public static Task<Dictionary<string, object>> Refund(RefundData refundData)
        {
            return Refund(refundData.AsParams());
        }

        public static Task<Dictionary<string, object>> GetRefundStatus(string mrefundid)
        {
            // Doc: https://developer.zalopay.vn/v2/general/overview.html#truy-van-trang-thai-hoan-tien-queryrefund
            var data = new Dictionary<string, string>();
            data.Add("app_id", _config["Thanh-toan:ZaloPay:AppID"]!);
            data.Add("m_refund_id", mrefundid);
            data.Add("timestamp", Utils.GetTimeStamp().ToString());
            data.Add("mac", ZaloPayMacGenerator.GetRefundStatus(data));

            return HttpHelper.PostFormAsync(_config["Thanh-toan:ZaloPay:Endpoint:GetRefundStatus"], data);
        }
    }
}