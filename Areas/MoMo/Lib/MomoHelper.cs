namespace NhaXeMaiLinh.Areas.MoMo.Lib
{
    public class MomoHelper
    {
        public static string GenRequestID()
        {
            return MomoUtils.GetFormattedDate() + MomoUtils.GetTimeStamp();
        }
    }
}