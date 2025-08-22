using NhaXeMaiLinh.Areas.Zalopay.Lib;
using NhaXeMaiLinh.Areas.Zalopay.Lib.Model;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace NhaXeMaiLinh.Areas.Zalopay
{
    [Area("Zalopay")]
    public class ZalopayController : Controller
    {
        private IConfiguration _configuration;
        public ZalopayController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Doc: https://developer.zalopay.vn/v2/general/overview.html#tao-don-hang
        [Route("/thanh-toan/zalopay/")]
        [HttpGet]
        public async Task<IActionResult> Payment(int amount, string orderInfo, string orderId/*, List<Item> items*/)
        {
            string returnUrl = string.Concat(_configuration.GetValue<string>("WebUrl"), "/thanh-toan/zalopay/confirm");

            // Handle extra info
            var embedData = new EmbedData([], returnUrl, "", "", "");

            // Phai doi
            // Take this shit from Session or TempData["//List//"]
            var items = new List<Item>()
            { 
                new("123", "test", 999999, 1)
            };

            // Handle to switch to another case of full constructor (check by input value)
            var orderData = new OrderData(amount, orderInfo, embedData, items);
            var order = await ZaloPayHelper.CreateOrder(orderData);

            var returncode = Convert.ToInt32(order["return_code"]);
            if (returncode == 1)
            {
                TempData["orderID"] = orderId;
                return Redirect(order["order_url"].ToString()!);
            }
            else
            {
                return Redirect("/thanh-toan/that-bai/" + returncode);
            }
        }

        // Doc: https://developer.zalopay.vn/v2/general/overview.html#truy-van-trang-thai-thanh-toan-cua-don-hang
        [Route("/thanh-toan/zalopay/confirm")]
        public async Task<IActionResult> PaymentConfirm()
        {
            if (Request.QueryString.HasValue)
            {
                var data = new Dictionary<string, object>();
                var query = HttpUtility.ParseQueryString(Request.QueryString.Value);
                foreach (string key in query.AllKeys)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        //lấy toàn bộ dữ liệu trả về và nạp vào danh sách
                        data.Add(key, query[key]);
                        // data[key] = query[key];
                    }
                }

                // Doc: https://developer.zalopay.vn/v2/general/overview.html#truy-van-trang-thai-thanh-toan-cua-don-hang_dac-ta-api_du-lieu-truyen-vao-api
                var order = await ZaloPayHelper.GetOrderStatus(data["apptransid"].ToString());

                // Doc: https://developer.zalopay.vn/v2/general/overview.html#truy-van-trang-thai-thanh-toan-cua-don-hang_dac-ta-api_tham-so-api-tra-ve
                var responseCode = Convert.ToInt32(order["return_code"]);
                if (responseCode == 1)
                {
                    // Lấy từ Session hoặc tự chèn vào SortedList trong Controller này
                    // Bổ sung thông tin từ "order"
                    TempData["orderId"] = TempData["orderID"];
                    TempData["transId"] = order["zp_trans_id"].ToString();

                    //Thanh toán thành công
                    return LocalRedirect("/thanh-toan/thanh-cong");
                }
                else
                {
                    //Thanh toán không thành công. Mã lỗi: responseCode
                    return LocalRedirect("/thanh-toan/that-bai" + responseCode);
                }
            }

            //phản hồi không hợp lệ
            return LocalRedirect("/thanh-toan/that-bai");
        }

        // Doc: https://developer.zalopay.vn/v2/general/overview.html#hoan-tien-giao-dich
        [Route("/hoan-tien/zalopay/")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refund(long amount, string zptransid, string description)
        {
            try
            {
                var refundData = new RefundData(amount, zptransid, description);
                var result = await ZaloPayHelper.Refund(refundData);

                // Doc: https://developer.zalopay.vn/v2/general/overview.html#hoan-tien-giao-dich_dac-ta-api_tham-so-api-tra-ve
                var returncode = int.Parse(result["return_code"].ToString());
                if (returncode >= 1)
                {
                    while (true)
                    {
                        var refundStatus = await ZaloPayHelper.GetRefundStatus(refundData.m_refund_id);
                        var c = int.Parse(refundStatus["return_code"].ToString());

                        if (c < 2)
                        {
                            if (c == 1)
                            {
                                // Thành công
                                // Save to db
                                return LocalRedirect("/hoan-tien/thanh-cong");
                            }

                            // Thất bại
                            LocalRedirect("/hoan-tien/that-bai" + c);
                            break;
                        }

                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    // Lỗi hệ thống
                    return LocalRedirect("/hoan-tien/that-bai/");
                }
            }
            catch (Exception)
            {
                // Lỗi hệ thống
                return LocalRedirect("/hoan-tien/that-bai/");
            }

            return LocalRedirect("/hoan-tien/that-bai/");
        }
    }
}
