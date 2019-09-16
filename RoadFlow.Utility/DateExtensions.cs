using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Utility
{
    public static class DateExtensions
    {
        /// <summary>
        /// 得到当前时间
        /// </summary>
        public static DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }

        public static DateTime MaxValue
        {
            get
            {
                return DateTime.MaxValue;
            }
        }

        public static DateTime MinValue
        {
            get
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 格式化为长日期格式(yyyy年M月d日)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToLongDate(this DateTime date)
        {
            return date.ToString("yyyy年M月d日");
        }

        /// <summary>
        /// 格式化为日期格式(yyy-MM-dd)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToDateString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 格式化为日期时间格式(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="date"></param>
        public static string ToDateTimeString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 格式化为日期时间格式(yyyy-MM-dd HH:mm)
        /// </summary>
        /// <param name="date"></param>
        public static string ToShortDateTimeString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 格式化为日期时间格式(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="date"></param>
        public static string ToDateTimeString(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToDateTimeString() : string.Empty;
        }

        /// <summary>
        /// 取日期时间的日期部分
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetDate(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd").ToDateTime();
        }

        
    }
}
