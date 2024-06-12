namespace NMap.Scanner
{
    public static class StringExtensions
    {
        public static string Capitalize(this string S)
        {
            if (string.IsNullOrEmpty(S))
            {
                return string.Empty;
            }

            var A = S.ToCharArray();
            A[0] = char.ToUpper(A[0]);

            return new string(A);
        }
    }
}