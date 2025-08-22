using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace NhaXeMaiLinh.Areas.VNPay
{
    public class VNPayCompare : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }

    public class VNPayUtils
    {
        public static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        public static string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;
                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();
                    return ipAddress;
                }
            }
            catch (Exception ex)
            {
                return "Invalid IP: " + ex.Message;
            }
            return "127.0.0.1";
        }
    }

    public class VNPayLib
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VNPayCompare());
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VNPayCompare());
        private readonly SortedList<string, string> _refundData = new SortedList<string, string>(new VNPayCompare());

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
        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            var data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }
            string queryString = data.ToString();

            baseUrl += "?" + queryString;
            string signData = queryString;
            if (signData.Length > 0)
            {
                signData = signData.Remove(data.Length - 1, 1);
            }
            string vnp_SecureHash = VNPayUtils.HmacSHA512(vnp_HashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

            return baseUrl;
        }
        #endregion

        #region Refund
        public async Task<Dictionary<string, object>> CreateRefundUrl(string baseUrl, string vnp_HashSecret)
        {
            var data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _refundData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Value) + "|");
                }
            }
            string queryString = data.ToString();

            if (queryString.Length > 0)
            {
                queryString = queryString.Remove(data.Length - 1, 1);
            }

            string vnp_SecureHash = VNPayUtils.HmacSHA512(vnp_HashSecret, queryString);
            _refundData.Add("vnp_SecureHash", vnp_SecureHash);

            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(baseUrl, _refundData);
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString)!;
            }
        }

        public bool ValidateChecksum(string inputHash, string secretKey, Dictionary<string, object> data)
        {
            string rspraw = GetRefundData(data);
            string myChecksum = VNPayUtils.HmacSHA512(secretKey, rspraw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        public string GetRefundData(Dictionary<string, object> query)
        {
            var data = new StringBuilder();
            if (query.ContainsKey("vnp_SecureHash"))
            {
                query.Remove("vnp_SecureHash");
            }

            foreach (var (key, value) in _refundData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                data.Append(value + "|");
            }

            if (data.Length > 0)
            {
                data = data.Remove(data.Length - 1, 1);
            }

            return data.ToString();
        }
        #endregion

        #region Response
        public bool ValidateSignature(string inputHash, string secretKey)
        {
            string rspraw = GetResponseData();
            string myChecksum = VNPayUtils.HmacSHA512(secretKey, rspraw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetResponseData()
        {
            var data = new StringBuilder();
            if (_responseData.ContainsKey("vnp_SecureHashType"))
            {
                _responseData.Remove("vnp_SecureHashType");
            }

            if (_responseData.ContainsKey("vnp_SecureHash"))
            {
                _responseData.Remove("vnp_SecureHash");
            }

            foreach (var (key, value) in _responseData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            //remove last '&'
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }

            return data.ToString();
        }
        #endregion
    }
}