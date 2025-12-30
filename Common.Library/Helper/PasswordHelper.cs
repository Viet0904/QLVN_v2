using System.Text;
using System.Security.Cryptography;

namespace Common.Library.Helper
{
    public class PasswordHelper
    {
        #region Properties

        private static string privateKey = "ThisProject";

        #endregion Properties

        #region private

        private static string createSHA1(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password + privateKey);
            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);

            var sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        #endregion private

        public static string CreatePassword(string password)
        {
            return createSHA1(password);
        }
    }
}
