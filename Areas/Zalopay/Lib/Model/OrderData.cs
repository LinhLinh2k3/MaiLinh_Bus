using Newtonsoft.Json;
using NhaXeMaiLinh.Areas.Zalopay.Lib.Crypto;

namespace NhaXeMaiLinh.Areas.Zalopay.Lib.Model
{
    // Docs: https://developer.zalopay.vn/v2/general/overview.html
    public class OrderData
    {
        private static IConfigurationRoot _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

        public int app_id { get; set; }
        public string app_user { get; set; } // id / username / tên / số điện thoại / email của user
        public string app_trans_id { get; set; }
        public long app_time { get; set; }
        public long? expire_duration_seconds { get; set; } // giá trị nhỏ nhất: 300, giá trị lớn nhất: 2592000
        public long amount { get; set; }
        public string item { get; set; } // JSON Array String
        public string description { get; set; }
        public string embed_data { get; set; } // JSON String
        public string bank_code { get; set; }
        public string mac { get; set; }
        public string? callback_url { get; set; } // This man is always NULL
        public string? device_info { get; set; } // JSON String
        public string? sub_app_id { get; set; } // Backup for appId only
        public string? title { get; set; }
        public string? currency { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }
        public string? address { get; set; }

        public OrderData(long amount, string description, EmbedData embeddata, List<Item> items)
        {
            app_id = Convert.ToInt32(_config["Thanh-toan:ZaloPay:AppID"]);
            app_user = "demo";
            app_trans_id = ZaloPayHelper.GenTransID();
            app_time = Utils.GetTimeStamp();
            expire_duration_seconds = null;
            this.amount = amount;
            item = JsonConvert.SerializeObject(items, Formatting.Indented);
            this.description = description;
            embed_data = JsonConvert.SerializeObject(embeddata);
            bank_code = "";
            mac = ComputeMac();
            callback_url = "";
            device_info = "";
            sub_app_id = "";
            title = "";
            currency = "";
            phone = "";
            email = "";
            address = "";
        }

        public OrderData(string appuser, long? expireDuration, long amount, string description, string? bankCode, string? callbackUrl, string? deviceInfo, string? subAppId,
            string? title, string? currency, string? phone, string? email, string? address, EmbedData embeddata, List<Item> items)
        {
            app_id = Convert.ToInt32(_config["Thanh-toan:ZaloPay:AppID"]);
            app_user = appuser;
            app_trans_id = ZaloPayHelper.GenTransID();
            app_time = Utils.GetTimeStamp();
            expire_duration_seconds = expireDuration ?? null;
            this.amount = amount;
            item = JsonConvert.SerializeObject(items, Formatting.Indented);
            this.description = description;
            embed_data = JsonConvert.SerializeObject(embeddata);
            bank_code = bankCode ?? "";
            mac = ComputeMac();
            callback_url = callbackUrl ?? "";
            device_info = JsonConvert.SerializeObject(deviceInfo) ?? "";
            sub_app_id = subAppId ?? "";
            this.title = title ?? "";
            this.currency = currency ?? "";
            this.phone = phone ?? "";
            this.email = email ?? "";
            this.address = address ?? "";
        }

        public virtual string GetMacData()
        {
            return app_id + "|" + app_trans_id + "|" + app_user + "|" + amount + "|" + app_time + "|" + embed_data + "|" + item;
        }

        public string ComputeMac()
        {
            return HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, _config["Thanh-toan:ZaloPay:Key1"]!, GetMacData());
        }
    }
}