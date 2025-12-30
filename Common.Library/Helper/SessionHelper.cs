using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Helper
{
    public class SessionHelper
    {
        public static string UserId { get; set; } = string.Empty;

        public static string UserName { get; set; } = string.Empty;
        public static string DonViSuDungId { get; set; } = string.Empty;
        public static string HoTen { get; set; } = string.Empty;

        public static DateTime NgayLamViec { get; set; }
        public static string PasswordApp { get; set; } = string.Empty;
        public static string DonViSuDungName { get; set; } = string.Empty;
    }
}
