namespace Assets.Scripts
{
    public static class StringExtensions
    {
        public static string UppercaseFirst(this string str)
        {
            // Check for empty string
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            
            // Return char and concat substring
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
