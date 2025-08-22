#nullable disable
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace NhaXeMaiLinh.Areas.VNPay
{
    [Area("VNPay")]
    public class VNPayController : Controller
    {
        private IConfiguration _configuration;
        public VNPayController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("/thanh-toan/vnpay")]
        [HttpGet]
        public IActionResult Payment(int amount, string orderInfo, string orderId)
        {
            string tmnCode = _configuration.GetValue<string>("Thanh-toan:VNPay:Vnp_TmnCode");
            string hashSecret = _configuration.GetValue<string>("Thanh-toan:VNPay:Vnp_HashSecret");
            string url = _configuration.GetValue<string>("Thanh-toan:VNPay:Vnp_Url");
            string returnUrl = string.Concat(_configuration.GetValue<string>("WebUrl"), "/thanh-toan/vnpay/confirm");
            string clientIPAddress = VNPayUtils.GetIpAddress(HttpContext);
            var now = DateTime.Now;
            string createDate = now.ToString("yyyyMMddHHmmss");
            string expireDate = now.AddMinutes(20).ToString("yyyyMMddHHmmss");

            VNPayLib payLib = new();
            payLib.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            payLib.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            payLib.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            payLib.AddRequestData("vnp_Amount", (amount * 100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            payLib.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            payLib.AddRequestData("vnp_CreateDate", createDate); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            payLib.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            payLib.AddRequestData("vnp_IpAddr", clientIPAddress); //Địa chỉ IP của khách hàng thực hiện giao dịch
            payLib.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            payLib.AddRequestData("vnp_OrderInfo", orderInfo); //Thông tin mô tả nội dung thanh toán
            payLib.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            payLib.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            payLib.AddRequestData("vnp_ExpireDate", expireDate); // 20 phút để thanh toán
            payLib.AddRequestData("vnp_TxnRef", orderId); //mã hóa đơn

            string paymentUrl = payLib.CreateRequestUrl(url, hashSecret);
            return Redirect(paymentUrl);
        }

        [Route("/thanh-toan/vnpay/confirm")]
        public IActionResult PaymentConfirm()
        {
            if (Request.QueryString.HasValue)
            {
                VNPayLib payLib = new();
                var query = HttpUtility.ParseQueryString(Request.QueryString.Value);
                foreach (string key in query.AllKeys)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        payLib.AddResponseData(key, query[key]); //lấy toàn bộ dữ liệu trả về và nạp vào danh sách
                    }
                }

                string tmnCode = _configuration.GetValue<string>("Payments:VNPay:Vnp_TmnCode");
                string hashSecret = _configuration.GetValue<string>("Payments:VNPay:Vnp_HashSecret");

                string orderId = payLib.GetResponseData("vnp_TxnRef").ToString(); //mã hóa đơn
                string orderInfo = payLib.GetResponseData("vnp_OrderInfo").ToString(); //Thông tin giao dịch
                long tranId = Convert.ToInt64(payLib.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                DateTime payDate = DateTime.ParseExact(payLib.GetResponseData("vnp_PayDate"), "yyyyMMddHHmmss", null); //thời gian giao dịch
                string responseCode = payLib.GetResponseData("vnp_ResponseCode").ToString(); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string secureHash = payLib.GetResponseData("vnp_SecureHash").ToString(); //hash của dữ liệu trả về

                bool checkSignature = payLib.ValidateSignature(secureHash, hashSecret);
                if (checkSignature && tmnCode == payLib.GetResponseData("vnp_TmnCode").ToString())
                {
                    if (responseCode != "00" || responseCode == null)
                    {
                        //Thanh toán không thành công. Mã lỗi: responseCode
                        return LocalRedirect("/thanh-toan/that-bai" + responseCode);
                    }

                    TempData["orderId"] = orderId;
                    TempData["transId"] = tranId;

                    //Thanh toán thành công
                    return LocalRedirect("/thanh-toan/thanh-cong");
                }
            }

            //phản hồi không hợp lệ
            return LocalRedirect("/thanh-toan/that-bai");
        }

        [Route("/hoan-tien/vnpay")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefundAsync(string transactionType, int amount, string orderId, string refundInfo, string orderDate, string userName)
        {
            string tmnCode = _configuration.GetValue<string>("Thanh-toan:VNPay:Vnp_TmnCode");
            string hashSecret = _configuration.GetValue<string>("Thanh-toan:VNPay:Vnp_HashSecret");
            string url = _configuration.GetValue<string>("Thanh-toan:VNPay:Refund");
            string clientIPAddress = VNPayUtils.GetIpAddress(HttpContext);
            string createDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            string generatedUID = Guid.NewGuid().ToString();

            VNPayLib payLib = new();
            payLib.AddRefundData("vnp_RequestId", generatedUID);
            payLib.AddRefundData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            payLib.AddRefundData("vnp_Command", "refund"); //Mã API sử dụng, mã cho giao dịch hoàn tiền là 'refund'
            payLib.AddRefundData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            payLib.AddRefundData("vnp_TransactionType", transactionType); //Loại giao dịch tại hệ thống VNPAY: 02: Giao dịch hoàn trả toàn phần(vnp_TransactionType= 02) 03: Giao dịch hoàn trả một phần(vnp_TransactionType= 03)
            payLib.AddRefundData("vnp_TxnRef", orderId); //Mã hóa đơn thanh toán (đặt hàng)
            payLib.AddRefundData("vnp_Amount", (amount * 100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            payLib.AddRefundData("vnp_OrderInfo", refundInfo); //Thông tin mô tả nội dung hoàn tiền
            payLib.AddRefundData("vnp_TransactionDate", orderDate); //ngày  thanh toán hóa đơn theo định dạng yyyyMMddHHmmss
            payLib.AddRefundData("vnp_CreateBy", userName); //tên người thực hiện hoàn tiền
            payLib.AddRefundData("vnp_CreateDate", createDate); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            payLib.AddRefundData("vnp_IpAddr", clientIPAddress); //Địa chỉ IP của khách hàng thực hiện giao dịch

            var refundResponse = await payLib.CreateRefundUrl(url, hashSecret);

            var responseCode = refundResponse["vnp_ResponseCode"].ToString();
            string secureHash = refundResponse["vnp_SecureHash"].ToString();
            bool isValid = payLib.ValidateChecksum(secureHash ,hashSecret, refundResponse);

            if (isValid)
            {
                if (responseCode == "00")
                {
                    return LocalRedirect("/hoan-tien/thanh-cong");
                }
                else
                {
                    //Thanh toán không thành công. Mã lỗi: responseCode
                    return LocalRedirect("/hoan-tien/that-bai/" + responseCode);
                }
            }

            //phản hồi không hợp lệ
            return LocalRedirect("/hoan-tien/that-bai/signature");
        }
    }
}