using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DouImp
{
    public class Transform
    {
        /// <summary>
        /// 年資計算
        /// </summary>
        /// <param name="dt1">任職起</param>
        /// <param name="dt2">任職迄</param>
        /// <returns></returns>
        public static string Seniority(DateTime dt1, DateTime dt2)
        {
            string result = "";

            try
            {
                int Year = dt2.Year - dt1.Year;
                int Month = (dt2.Year - dt1.Year) * 12 + (dt2.Month - dt1.Month);
                //2端月份皆要加入計算
                Month = Month + 1;
                result = Math.Round(Convert.ToDouble(Month) / 12, 1).ToString();
            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }
    }
}