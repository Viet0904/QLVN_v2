using Common.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Helper
{
    public class DateTimeHelper
    {
        public static DateTime ToDateTime(object dateTime)
        {
            DateTime result = DateTime.MinValue;
            if (dateTime != null)
            {
                DateTime.TryParse(dateTime.ToString(), out result);
            }

            return result;
        }

        public static string ToString(DateTime? dateTime)
        {
            return ToDateTime(dateTime).ToString(FormatConstant.DateTime);
        }

        public static DateTime GetLastDayOfMonth(int iMonth)
        {
            DateTime dtResult = new DateTime(DateTime.Now.Year, iMonth, 1);
            dtResult = dtResult.AddMonths(1);
            dtResult = dtResult.AddDays(-(dtResult.Day));
            return dtResult;
        }

        /// <summary>
        /// Lấy ra ngày đầu tiên trong tháng được truyền vào
        /// là 1 số nguyên từ 1 đến 12
        /// </summary>
        /// <param name="iMonth">Thứ tự của tháng trong năm</param>
        /// <returns>Ngày đầu tiên trong tháng</returns>
        public static DateTime GetFirstDayOfMonth(int iMonth, int iYear)
        {
            DateTime dtResult = new DateTime(iYear, iMonth, 1);
            dtResult = dtResult.AddDays((-dtResult.Day) + 1);
            return dtResult;
        }
        public static DateTime GetFirstDayOfMonth(DateTime dtInput)
        {
            DateTime dtResult = dtInput;
            dtResult = dtResult.AddDays((-dtResult.Day) + 1);
            return dtResult;
        }
        public static DateTime GetLastDayOfMonth(DateTime dtInput)
        {
            DateTime dtResult = dtInput;
            dtResult = dtResult.AddMonths(1);
            dtResult = dtResult.AddDays(-(dtResult.Day));
            return dtResult;
        }
        public static DateTime GetFirstDayOfMonth_TruDay(int iDay, int iMonth, int iYear, int SoTru, DateTime dateTime)
        {
            DateTime dtResult = new DateTime(iYear, iMonth, iDay);
            dtResult = dtResult.AddDays(-SoTru);
            if (dtResult.Date < dateTime.Date)
                return dateTime;
            else
                return dtResult;
        }
    }
}
