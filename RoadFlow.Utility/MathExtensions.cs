using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Utility
{
    public static class MathExtensions
    {
        /// <summary>
        /// 判断数字是否在参数里面
        /// </summary>
        /// <param name="digit"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static bool In(this int digit, params int[] digits)
        {
            foreach (int i in digits)
            {
                if (i == digit)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断数字不在参数里面
        /// </summary>
        /// <param name="digit"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static bool NotIn(this int digit, params int[] digits)
        {
            foreach (int i in digits)
            {
                if (i == digit)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 转换为文件大小显示
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ToFileSize(this long size)
        {
            String fileSize = string.Empty;
            if (size < 1024)
            {
                fileSize = size + "BT";
            }
            else if (size < 1048576)
            {
                fileSize = ((double)size / 1024).ToString("0.00") + "KB";
            }
            else if (size < 1073741824)
            {
                fileSize = ((double)size / 1048576).ToString("0.00") + "MB";
            }
            else
            {
                fileSize = ((double)size / 1073741824).ToString("0.00") + "GB";
            }
            return fileSize;
        }
    }
}
