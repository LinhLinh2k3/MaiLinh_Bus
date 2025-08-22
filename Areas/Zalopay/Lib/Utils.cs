namespace NhaXeMaiLinh.Areas.Zalopay.Lib
{
    public class Utils
    {
        public static long GetTimeStamp()
        {
            // Lấy thời gian hiện tại theo múi giờ Việt Nam (GMT+7)
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            // Chuyển đổi thời gian hiện tại sang Unix timestamp (tính đến mili giây)
            long unixTimestamp = new DateTimeOffset(vietnamTime).ToUnixTimeMilliseconds();
            return unixTimestamp;
        }

        public static string GetFormattedDate()
        {
            // Get the current time in UTC
            DateTime utcNow = DateTime.UtcNow;
            // Define the Vietnam timezone (GMT+7)
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            // Convert the current time to Vietnam timezone
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
            // Format the date as yymmdd
            string formattedDate = vietnamTime.ToString("yyMMdd");
            return formattedDate;
        }
    }
}