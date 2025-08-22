using NhaXeMaiLinh.Areas.MoMo.Lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Web;

namespace NhaXeMaiLinh.Areas.MoMo
{
    [Area("MoMo")]
    public class MoMoController : Controller
    {
        private IConfiguration _configuration;
        public MoMoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Doc: https://developers.momo.vn/v2/#/docs/aiov2/?id=l%e1%ba%a5y-ph%c6%b0%c6%a1ng-th%e1%bb%a9c-thanh-to%c3%a1n
        [Route("/thanh-toan/Momo")]
        [HttpGet]
        public async Task<IActionResult> PaymentAsync(long amount, string orderId, string orderInfo)
        {
            string partnerCode = _configuration.GetValue<string>("Thanh-toan:MoMo:PartnerCode");
            string returnUrl = string.Concat(_configuration.GetValue<string>("WebsiteURL"), "/payment/momo/confirm");
            string url = _configuration.GetValue<string>("Thanh-toan:MoMo:Endpoint:CreateUrl");
            string secretKey = _configuration.GetValue<string>("Thanh-toan:MoMo:SecretKey");

            MomoLib paylib = new MomoLib();
            paylib.AddRequestData("partnerCode", partnerCode);
            paylib.AddRequestData("requestId", MomoHelper.GenRequestID()); // Định danh mỗi yêu cầu
            paylib.AddRequestData("amount", amount.ToString()); // Số tiền cần thanh toán | Min: 1.000 VND | Max: 20.000.000 VND
            paylib.AddRequestData("orderId", orderId); // Mã đơn hàng thanh toán
            paylib.AddRequestData("orderInfo", orderInfo); // Thông tin đơn hàng
            paylib.AddRequestData("redirectUrl", returnUrl);
            paylib.AddRequestData("ipnUrl", returnUrl); // IPN thành returnURL (Không có server Web API)
            paylib.AddRequestData("requestType", "payWithATM"); // Mặc định: "captureWallet", ATM: "payWithATM"
            paylib.AddRequestData("extraData", ""); // Thông tin bổ sung, JSON string
            paylib.AddRequestData("lang", "vi");

            // Optional
            string partnerName = _configuration.GetValue<string>("Thanh-toan:MoMo:PartnerName");
            paylib.AddRequestData("partnerName", !partnerName.IsNullOrEmpty() ? partnerName : "");
            string storeId = _configuration.GetValue<string>("Thanh-toan:MoMo:StoreID");
            paylib.AddRequestData("storeId", !storeId.IsNullOrEmpty() ? storeId : "");

            var paymentResponse = await paylib.CreateRequestUrl(url, secretKey);
            var resultCode = int.Parse(paymentResponse["resultCode"].ToString());

            if (resultCode == 0)
            {
                return Redirect(paymentResponse["payUrl"].ToString());
            }
            else
            {
                return Redirect("/thanh-toan/that-bai/" + resultCode);
            }
        }

        // Doc: https://developers.momo.vn/v2/#/docs/aiov2/?id=giao-di%e1%bb%87n-redirect
        // Doc: https://developers.momo.vn/v2/#/docs/aiov2/?id=th%c3%b4ng-tin-tham-s%e1%bb%91
        [Route("/thanh-toan/Momo/confirm")]
        public IActionResult PaymentConfirm()
        {
            if (Request.QueryString.HasValue)
            {
                MomoLib momoLib = new MomoLib();
                var query = HttpUtility.ParseQueryString(Request.QueryString.Value);
                foreach (string key in query.AllKeys)
                {
                    momoLib.AddResponseData(key, query[key]); //lấy toàn bộ dữ liệu trả về và nạp vào danh sách
                }

                string secretKey = _configuration.GetValue<string>("Thanh-toan:MoMo:SecretKey");
                string signature = momoLib.GetResponseData("signature");
                int resultCode = int.Parse(momoLib.GetResponseData("resultCode"));
                string resultMsg = momoLib.GetResponseData("message");

                string orderId = momoLib.GetResponseData("orderId");
                string requestId = momoLib.GetResponseData("requestId");
                long amount = long.Parse(momoLib.GetResponseData("amount"));
                string transId = momoLib.GetResponseData("transId");

                bool isValid = momoLib.ValidateSignature(signature ,secretKey);
                if (isValid)
                {
                    if (resultCode == 0)
                    {
                        TempData["orderId"] = orderId;
                        TempData["transId"] = transId;

                        return LocalRedirect("/thanh-toan/thanh-cong");
                    }
                    else
                    {
                        //Thanh toán không thành công
                        return LocalRedirect("/thanh-toan/that-bai" + resultCode);
                    }
                }

                //Thanh toán không thành công
                return LocalRedirect("/thanh-toan/that-bai/");
            }
            return View();
        }

        // Doc: https://developers.momo.vn/v2/#/docs/aiov2/?id=ho%c3%a0n-ti%e1%bb%81n-giao-d%e1%bb%8bch
        [Route("/hoan-tien/momo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefundAsync(int amount, string transId, string refundInfo = null)
        {
            string partnerCode = _configuration.GetValue<string>("Thanh-toan:MoMo:PartnerCode");
            string url = _configuration.GetValue<string>("Thanh-toan:MoMo:Endpoint:RefundUrl");
            string secretKey = _configuration.GetValue<string>("Thanh-toan:MoMo:SecretKey");

            MomoLib momoLib = new MomoLib();
            momoLib.AddRefundData("partnerCode", partnerCode);
            momoLib.AddRefundData("orderId", MomoHelper.GenRequestID());
            momoLib.AddRefundData("requestId", MomoHelper.GenRequestID());
            momoLib.AddRefundData("amount", amount.ToString());
            momoLib.AddRefundData("transId", transId);
            momoLib.AddRefundData("lang", "vi");
            momoLib.AddRefundData("description", refundInfo ?? "");

            var responseResult = await momoLib.CreateRefundUrl(url, secretKey);
            var resultCode = int.Parse(responseResult["resultCode"].ToString());
            var resultMsg = responseResult["message"].ToString();

            if (resultCode == 0)
            {
                return LocalRedirect("/hoan-tien/thanh-cong");
            }

            return LocalRedirect("/hoan-tien/that-bai/" + resultCode);
        }
    }
}
