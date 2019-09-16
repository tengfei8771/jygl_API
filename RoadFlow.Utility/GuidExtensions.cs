using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Utility
{
    public static class GuidExtensions
    {
        /// <summary>
        /// 判断为空GUID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsEmptyGuid(this Guid guid)
        {
            return guid == Guid.Empty;
        }
        /// <summary>
        /// 判断为空GUID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsEmptyGuid(this Guid? guid)
        {
            return !guid.HasValue || guid.Value == Guid.Empty;
        }
        /// <summary>
        /// 判断不为空GUID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsNotEmptyGuid(this Guid guid)
        {
            return guid != Guid.Empty;
        }
        /// <summary>
        /// 判断不为NULL和空GUID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsNotEmptyGuid(this Guid? guid)
        {
            return guid.HasValue && guid.Value != Guid.Empty;
        }
        /// <summary>
        /// 将GUID转换为整数
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static int ToInt(this Guid guid)
        {
            return Math.Abs(guid.GetHashCode());
        }

        public static string ToUpperString(this Guid guid)
        {
            return guid.ToString().ToUpper();
        }

        public static string ToLowerString(this Guid guid)
        {
            return guid.ToString().ToLower();
        }

        /// <summary>
        /// 没有分隔线的字符串
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToNString(this Guid guid)
        {
            return guid.ToString("N");
        }

        /// <summary>
        /// 没有分隔线的小写字符串
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToLowerNString(this Guid guid)
        {
            return guid.ToString("N").ToLower();
        }

        /// <summary>
        /// 没有分隔线的大写字符串
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToUpperNString(this Guid guid)
        {
            return guid.ToString("N").ToUpper();
        }
    }
}
