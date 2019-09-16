using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Utility
{
    /// <summary>
    /// 手机短信类
    /// </summary>
    public class SMS
    {
        /// <summary>
        /// 发送手机短信
        /// </summary>
        /// <param name="contents">内容</param>
        /// <param name="mobileNumber">手机号码(多个用逗号分开)</param>
        /// <returns></returns>
        public static bool SendSMS(string contents, string mobileNumbers)
        {
            if (contents.IsNullOrWhiteSpace())
            {
                return false;
            }
            string[] numbers = mobileNumbers.Split(',');
            foreach (string number in numbers)
            {

            }
            return true;
        }
    }
}
