﻿using System.Text.RegularExpressions;

namespace RoadFlow.Pinyin
{
    /// <summary>
    /// 拼音工具类
    /// </summary>
    public static class PinyinUtil
    {
        /// <summary>
        /// 判断字符是否是汉字
        /// </summary>
        /// <param name="ch">要判断的字符</param>
        /// <returns></returns>
        public static bool IsHanzi(char ch)
        {
            return Regex.IsMatch(ch.ToString(), @"[\u4e00-\u9fbb]");
        }
        
    }
}
