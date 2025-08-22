namespace NhaXeMaiLinh.Services
{
    public class Generator
    {
        public static string GenOrderId()
        {
            return Guid.NewGuid().ToString();
        }

        public static string Capitalize(string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                throw new ArgumentException("String is mull or empty");
            }

            return string.Concat(s[0].ToString().ToUpper(), s.AsSpan(1));
        }
    }
}
