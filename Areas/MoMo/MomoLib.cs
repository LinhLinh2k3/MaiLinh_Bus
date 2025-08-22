using NhaXeMaiLinh.Areas.MoMo.Lib;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace NhaXeMaiLinh.Areas.MoMo
{
    public class MomoCompare : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var compare = CompareInfo.GetCompareInfo("en-US");
            return compare.Compare(x, y, CompareOptions.Ordinal);
        }
    }

    public class MomoLib
    {
        private static IConfigurationRoot _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new MomoCompare());
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new MomoCompare());
        private readonly SortedList<string, string> _refundData = new SortedList<string, string>(new MomoCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out var retValue) ? retValue : string.Empty;
        }

        public void AddRefundData(string key, string value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _refundData.Add(key, value);
            }
        }

        #region Request
        public async Task<Dictionary<string, object>> CreateRequestUrl(string baseUrl, string secretKey)
        {
            SortedList<string, string> tempData = new();
            foreach (var kyp in _requestData)
            {
                tempData[kyp.Key] = kyp.Value;
            }

            tempData.Add("accessKey", _config["Thanh-toan:MoMo:AccessKey"]!);
            tempData.Remove("lang");
            if (tempData.ContainsKey("partnerName")) tempData.Remove("partnerName");
            if (tempData.ContainsKey("storeId")) tempData.Remove("storeId");

            var data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in tempData)
            {
                data.Append(string.Concat(kv.Key, "=", kv.Value, "&"));
            }

            string signData = data.ToString();
            if (signData.Length > 0)
            {
                signData = signData.Remove(data.Length - 1, 1);
            }

            string signature = MomoUtils.HmacSHA256(secretKey, signData);
            AddRequestData("signature", signature);

            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(baseUrl, _requestData);
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString)!;
            }
        }
        #endregion

        #region Response
        public bool ValidateSignature(string inputHash, string secretKey)
        {
            string rspraw = GetResponseData();
            string myChecksum = MomoUtils.HmacSHA256(secretKey, rspraw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetResponseData()
        {
            SortedList<string, string> tempData = new();
            foreach (var kyp in _responseData)
            {
                tempData[kyp.Key] = kyp.Value;
            }
            tempData.Add("accessKey", _config["Thanh-toan:MoMo:AccessKey"]!);
            tempData.Remove("signature");

            var data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in tempData)
            {
                data.Append(string.Concat(kv.Key, "=", kv.Value, "&"));
            }

            //remove last '&'
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }

            return data.ToString();
        }
        #endregion

        #region Refund
        public async Task<Dictionary<string, object>> CreateRefundUrl(string baseUrl, string secretKey)
        {
            SortedList<string, string> tempData = new();
            foreach (var kyp in _requestData)
            {
                tempData[kyp.Key] = kyp.Value;
            }

            tempData.Add("accessKey", _config["Thanh-toan:MoMo:AccessKey"]!);
            tempData.Remove("lang");

            var data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in tempData)
            {
                data.Append(string.Concat(kv.Key, "=", kv.Value, "&"));
            }

            string signData = data.ToString();
            if (signData.Length > 0)
            {
                signData = signData.Remove(data.Length - 1, 1);
            }

            string signature = MomoUtils.HmacSHA256(secretKey, signData);
            AddRefundData("signature", signature);

            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(baseUrl, _requestData);
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString)!;
            }
        }
        #endregion
    }
}