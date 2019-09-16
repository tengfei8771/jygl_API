using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace RoadFlow.Utility
{
    /// <summary>
    /// 字符串操作扩展类
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 判断字符串是否为null或""
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        /// <summary>
        /// 判断字符串是否为null或""或" "(包含空字符的字符串)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        /// <summary>
        /// 比较字符串区分大小写
        /// </summary>
        /// <param name="str1">字符串1</param>
        /// <param name="str2">字符串2</param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return null == str1 ? null == str2 : str1.Equals(str2, StringComparison.CurrentCultureIgnoreCase);
        }
        /// <summary>
        /// 判断字符串是否包含（不区分大不写）
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this string str1, string str2)
        {
            return null == str1 || null == str2 ? false : str1.IndexOf(str2, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
        /// <summary>
        /// 去除空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Trim1(this string str)
        {
            return str.IsNullOrEmpty() ? "" : str.Trim();
        }
        /// <summary>
        /// 将字符串转换为GUID
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string str)
        {
            return Guid.TryParse(str.Trim1(), out Guid guid) ? guid : Guid.Empty;
        }
        /// <summary>
        /// 判断一个字符串是否是GUID
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsGuid(this string str)
        {
            return Guid.TryParse(str.Trim1(), out Guid guid);
        }
        /// <summary>
        /// 判断一个字符串是否是GUID
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsGuid(this string str, out Guid guid)
        {
            return Guid.TryParse(str.Trim1(), out guid);
        }
        /// <summary>
        /// 判断一个字符串是否是字体图标(以fa开头)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsFontIco(this string str)
        {
            return str.Trim1().StartsWith("fa");
        }
        /// <summary>
        /// 判断字符串是否为整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(this string str)
        {
            return int.TryParse(str, out int i);
        }
        /// <summary>
        /// 判断字符串是否为整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(this string str, out int i)
        {
            return int.TryParse(str, out i);
        }
        /// <summary>
        /// 将字符串转换为整数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns></returns>
        public static int ToInt(this string str, int defaultValue = int.MinValue)
        {
            return int.TryParse(str, out int i) ? i : defaultValue;
        }
        /// <summary>
        /// 判断字符串是否为数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDecimal(this string str)
        {
            return decimal.TryParse(str, out decimal d);
        }
        /// <summary>
        /// 判断字符串是否为数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDecimal(this string str, out decimal d)
        {
            return decimal.TryParse(str, out d);
        }
        /// <summary>
        /// 将字符串转换为数字
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns></returns>
        public static decimal ToDecimal(this string str, decimal defaultValue = decimal.MinValue)
        {
            return decimal.TryParse(str, out decimal d) ? d : defaultValue;
        }
        /// <summary>
        /// 将字符串MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5(this string str)
        {
            return Encryption.MD5(str.Trim1());
        }
        /// <summary>
        /// 判断字符串是否为日期时间
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string str)
        {
            return DateTime.TryParse(str, out DateTime dt);
        }
        /// <summary>
        /// 判断字符串是否为日期时间
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string str, out DateTime dt)
        {
            return DateTime.TryParse(str, out dt);
        }
        /// <summary>
        /// 将字符串转换为日期时间
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str)
        {
            return DateTime.TryParse(str, out DateTime dt) ? dt : DateTime.MinValue;
        }
        /// <summary>
        /// 验证字符串是否为数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDigital(this string str)
        {
            foreach (char c in str.ToCharArray())
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 验证是否为固话号码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsTelNumber(this string str)
        {
            //去掉-线后全为数字
            return str.IsNullOrWhiteSpace() ? false : !str.StartsWith("-") && str.Replace("-", "").IsDigital();
        }

        /// <summary>
        /// 去掉组织机构人员前缀
        /// </summary>
        /// <returns></returns>
        public static string RemoveUserPrefix(this string str)
        {
            return str.IsNullOrWhiteSpace() ? "" : str.StartsWith("u_") ? str.TrimStart('u', '_') : str;
        }
        /// <summary>
        /// 去掉组织机构工作组前缀
        /// </summary>
        /// <returns></returns>
        public static string RemoveWorkGroupPrefix(this string str)
        {
            return str.IsNullOrWhiteSpace() ? "" : str.StartsWith("w_") ? str.TrimStart('w', '_') : str;
        }
        /// <summary>
        /// 去掉组织机构人员兼职前缀
        /// </summary>
        /// <returns></returns>
        public static string RemoveUserRelationPrefix(this string str)
        {
            return str.IsNullOrWhiteSpace() ? "" : str.StartsWith("r_") ? str.TrimStart('r', '_') : str;
        }

        /// <summary>
        /// 移出所有空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimAll(this string str)
        {
            return Regex.Replace(str, @"\s", "");
        }

        /// <summary>
        /// 转换为SQL的in字符串
        /// </summary>
        /// <param name="str">逗号分开的字符串</param>
        /// <param name="isSignle">是否加单引号</param>
        /// <returns></returns>
        public static string ToSqlIn(this string str, bool isSignle = true)
        {
            if (str.IsNullOrEmpty())
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string s in str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (isSignle)
                {
                    stringBuilder.Append("'");
                }
                stringBuilder.Append(s);
                if (isSignle)
                {
                    stringBuilder.Append("'");
                }
                stringBuilder.Append(",");
            }
            return stringBuilder.ToString().TrimEnd(',');
        }

        #region 得到汉字拼音
        /// <summary>
        /// 得到汉字拼音(全拼)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToPinYing(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }
            var format = new Pinyin.format.PinyinOutputFormat(Pinyin.format.ToneFormat.WITHOUT_TONE,
                Pinyin.format.CaseFormat.LOWERCASE, Pinyin.format.VCharFormat.WITH_U_AND_COLON);
            return Pinyin.Pinyin4Net.GetPinyin(str, format).TrimAll();
        }
        #endregion

        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlEncode(this string url)
        {
            return WebUtility.UrlEncode(url);
        }
        /// <summary>
        /// URL解码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlDecode(this string url)
        {
            return WebUtility.UrlDecode(url);
        }
        /// <summary>
        /// HTML编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string str)
        {
            return WebUtility.HtmlEncode(str);
        }
        /// <summary>
        /// HTML解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string str)
        {
            return WebUtility.HtmlDecode(str);
        }
        /// <summary>
        /// 将List拼接为字符串
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="split">分隔符</param>
        /// <param name="prefix">前缀</param>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public static string JoinList<T>(this IEnumerable<T> ts, string split = ",", string prefix = "", string suffix = "")
        {
            if (null == ts || !ts.Any())
            {
                return "";
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var t in ts)
            {
                stringBuilder.Append(prefix);
                stringBuilder.Append(t);
                stringBuilder.Append(suffix);
                stringBuilder.Append(split);
            }
            return stringBuilder.ToString().TrimEnd(split.ToCharArray());
        }
        /// <summary>
        /// 将List转换为SQL in语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="single">是包含单引号</param>
        /// <returns></returns>
        public static string JoinSqlIn<T>(this List<T> ts, bool single = true)
        {
            return ts.JoinList(",", single ? "'" : "", single ? "'" : "");
        }
        /// <summary>
        /// 得到实符串实际长度
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int Size(this string str)
        {
            byte[] strArray =Encoding.Default.GetBytes(str);
            return strArray.Length;
        }
        ///   <summary>   
        ///   去除HTML标记   
        ///   </summary>   
        ///   <param   name="NoHTML">包括HTML的源码   </param>   
        ///   <returns>已经去除后的文字</returns>   
        public static string RemoveHTML(this string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            
            return Htmlstring;
        }
        /// <summary>
        /// 过滤js脚本
        /// </summary>
        /// <param name="strFromText"></param>
        /// <returns></returns>
        public static string RemoveScript(this string html)
        {
            if (html.IsNullOrEmpty()) return string.Empty;
            Regex regex1 = new Regex(@"<script[\s\S]+</script *>", RegexOptions.IgnoreCase);
            Regex regex2 = new Regex(@" href *= *[\s\S]*script *:", RegexOptions.IgnoreCase);
            Regex regex3 = new Regex(@" on[\s\S]*=", RegexOptions.IgnoreCase);
            Regex regex4 = new Regex(@"<iframe[\s\S]+</iframe *>", RegexOptions.IgnoreCase);
            Regex regex5 = new Regex(@"<frameset[\s\S]+</frameset *>", RegexOptions.IgnoreCase);
            html = regex1.Replace(html, ""); //过滤<script></script>标记
            html = regex2.Replace(html, ""); //过滤href=javascript: (<A>) 属性
            html = regex3.Replace(html, " _disibledevent="); //过滤其它控件的on...事件
            html = regex4.Replace(html, ""); //过滤iframe
            html = regex5.Replace(html, ""); //过滤frameset
            return html;
        }
        /// <summary>
        /// 替换页面标签
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string RemovePageTag(this string html)
        {
            if (html.IsNullOrEmpty()) return string.Empty;
            Regex regex0 = new Regex(@"<!DOCTYPE[^>]*>", RegexOptions.IgnoreCase);
            Regex regex1 = new Regex(@"<html\s*", RegexOptions.IgnoreCase);
            Regex regex2 = new Regex(@"<head[\s\S]+</head\s*>", RegexOptions.IgnoreCase);
            Regex regex3 = new Regex(@"<body\s*", RegexOptions.IgnoreCase);
            Regex regex4 = new Regex(@"<form\s*", RegexOptions.IgnoreCase);
            Regex regex5 = new Regex(@"</(form|body|head|html)>", RegexOptions.IgnoreCase);
            html = regex0.Replace(html, ""); //过滤<html>标记
            html = regex1.Replace(html, "<html\u3000 "); //过滤<html>标记
            html = regex2.Replace(html, ""); //过滤<head>属性
            html = regex3.Replace(html, "<body\u3000 "); //过滤<body>属性
            html = regex4.Replace(html, "<form\u3000 "); //过滤<form>属性
            html = regex5.Replace(html, "</$1\u3000>"); //过滤</html></body></head></form>属性
            return html;
        }

        /// <summary>
        /// 取得html中的图片
        /// </summary>
        /// <param name="HTMLStr"></param>
        /// <returns></returns>
        public static string GetImg(this string text)
        {
            string str = string.Empty;
            Regex r = new Regex(@"<img\s+[^>]*\s*src\s*=\s*([']?)(?<url>\S+)'?[^>]*>", //注意这里的(?<url>\S+)是按正则表达式中的组来处理的，下面的代码中用使用到，也可以更改成其它的HTML标签，以同样的方法获得内容！
            RegexOptions.Compiled);
            Match m = r.Match(text.ToLower());
            if (m.Success)
                str = m.Result("${url}").Replace("\"", "").Replace("'", "");
            return str;
        }
        /// <summary>
        /// 取得html中的所有图片
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string[] GetImgs(this string text)
        {
            List<string> imgs = new List<string>();
            string pat = @"<img\s+[^>]*\s*src\s*=\s*([']?)(?<url>\S+)'?[^>]*>";
            Regex r = new Regex(pat, RegexOptions.Compiled);
            Match m = r.Match(text.ToLower());
            while (m.Success)
            {
                imgs.Add(m.Result("${url}").Replace("\"", "").Replace("'", ""));
                m = m.NextMatch();
            }
            return imgs.ToArray();
        }
        /// <summary>
        /// 替换字符串不区分大小写
        /// </summary>
        /// <param name="str"></param>
        /// <param name="oldStr"></param>
        /// <param name="newStr"></param>
        /// <returns></returns>
        public static string ReplaceIgnoreCase(this string str, string oldStr, string newStr)
        {
            return str.IsNullOrWhiteSpace() ? "" : Regex.Replace(str,
                oldStr.IsNullOrEmpty() ? "" : oldStr, newStr.IsNullOrEmpty() ? "" : newStr, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 判断子字符串位置，不区分大小写
        /// </summary>
        /// <param name="str"></param>
        /// <param name="subString"></param>
        /// <returns></returns>
        public static int IndexOfIgnoreCase(this string str, string subString)
        {
            return str.IndexOf(subString, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 过滤查询SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string FilterSelectSql(this string sql)
        {
            return sql.IsNullOrWhiteSpace() ? "" 
                : sql.ReplaceIgnoreCase("DELETE ", "")
                .ReplaceIgnoreCase("UPDATE ", "")
                .ReplaceIgnoreCase("INSERT ", "")
                .ReplaceIgnoreCase("TRUNCATE TABLE ", "")
                .ReplaceIgnoreCase("DROP ", "")
                .ReplaceIgnoreCase(@"\s+exec(\s|\+)+(s|x)p\w+", string.Empty);//防止执行sql server 内部存储过程或扩展存储过程
                
        }
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DESEncrypt(this string str)
        {
            return str.IsNullOrWhiteSpace() ? "" : Encryption.DESEncrypt(str);
        }
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DESDecrypt(this string str)
        {
            return str.IsNullOrWhiteSpace() ? "" : Encryption.DESDecrypt(str);
        }
        /// <summary>
        /// 以附件的形式显示字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isBr">是否换行</param>
        /// <param name="isIndex">是否显示序号</param>
        /// <returns></returns>
        public static string ToFilesShowString(this string str, bool isBr = true, bool isIndex = true)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return "";
            }
            StringBuilder stringBuilder = new StringBuilder();
            int index = 1;
            foreach (string file in str.Split('|'))
            {
                if (file.IsNullOrWhiteSpace())
                {
                    continue;
                }
                string fileName = System.IO.Path.GetFileName(file.DESDecrypt());
                stringBuilder.Append(isBr ? "<div style=\"margin-bottom:4px;\">" : "<span style=\"margin-right:4px;\">");
                stringBuilder.Append("<a class=\"blue1\" target=\"_blank\" href=\"/RoadFlowCore/Controls/ShowFile?file=" + file + "\">");
                stringBuilder.Append(isIndex ? index++.ToString() + "、" : string.Empty);
                stringBuilder.Append(fileName + "</a>");
                stringBuilder.Append(isBr ? "</div>" : "</span>");
                if (!isBr)
                {
                    stringBuilder.Append("；");
                }
            }
            return stringBuilder.ToString().TrimEnd('；');
        }

        /// <summary>
        /// 将附件显示为图片IMG
        /// </summary>
        /// <param name="str"></param>
        /// <param name="width">宽度 0表示不设置</param>
        /// <param name="height">高度 0表示不设置</param>
        /// <returns></returns>
        public static string ToFilesImgString(this string str, int width = 0, int height = 0)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return "";
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string file in str.Split('|'))
            {
                if (file.IsNullOrWhiteSpace())
                {
                    continue;
                }
                string fileName = System.IO.Path.GetFileName(file.DESDecrypt());
                stringBuilder.Append("<a target=\"_blank\" href=\"/RoadFlowCore/Controls/ShowFile?file=" + file + "\"><img border=\"0\" style=\"border:none 0;margin:3px 12px 3px 0;" + (width != 0 ? "width:" + width + "px;" : "") + (height != 0 ? "height:" + height + "px;" : "") + "\" src=\"/RoadFlowCore/Controls/ShowFile?file=" + file + "\"/></a>");
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 从字符串中提取数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetNumber(this string str)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in str.ToCharArray())
            {
                if (int.TryParse(c.ToString(), out int a))
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().TrimStart('0');
        }

        /// <summary>
        /// 截取字符串，汉字两个字节，字母一个字节
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="len">截取长度</param>
        /// <param name="show">截取后加上符号</param>
        /// <returns></returns>
        public static string CutOut(this string str, int len, string show = "…")
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(str);
            int oldLen = s.Length;
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                { tempLen += 2; }
                else
                { tempLen += 1; }
                try
                { tempString += str.Substring(i, 1); }
                catch
                { break; }
                if (tempLen > len) break;
            }
            //如果截过则加上半个省略号 
            if (oldLen > len)
                tempString += show;
            tempString = tempString.Replace("&nbsp;", " ");
            tempString = tempString.Replace("&lt;", "<");
            tempString = tempString.Replace("&gt;", ">");
            tempString = tempString.Replace('\n'.ToString(), "<br>");
            return tempString;
        }

        /// <summary>
        /// 截取副标题
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="len"></param>
        /// <param name="show"></param>
        /// <returns></returns>
        public static string CutSubTitle(this string contents, int len, string show = "…")
        {
            return contents.RemoveHTML().CutOut(len, show);
        }

        /// <summary>
        /// 字符串用逗号分开加上双引号(主要用于oracle查询)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AddDoubleQuotes(this string str)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            string[] strings = str.Split(',');
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string s in strings)
            {
                string oreder = s.ContainsIgnoreCase("asc") ? "ASC" : s.ContainsIgnoreCase("desc") ? "DESC" : "ASC";
                stringBuilder.Append("\"" + s.ReplaceIgnoreCase("asc", "").ReplaceIgnoreCase("desc", "").Trim1() + "\" " + oreder + ",");
            }
            return stringBuilder.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 判断一个字符串LIST中是否包含某个字符串，不区分大小写
        /// </summary>
        /// <param name="list"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this List<string> list, string str)
        {
            foreach (string s in list)
            {
                if (s.EqualsIgnoreCase(str))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 得到Request.Query字符串
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Querys(this HttpRequest request, string key)
        {
            string str = request.Query[key];
            return str ?? string.Empty;
        }
        /// <summary>
        /// 得到Request.Form字符串
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Forms(this HttpRequest request, string key)
        {
            string str = request.Form[key];
            return str ?? string.Empty;
        }
        /// <summary>
        /// 得到Request.QueryString
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string UrlQuery(this HttpRequest request)
        {
            QueryString queryString = request.QueryString;
            return queryString.HasValue ? queryString.Value : string.Empty;
        }
        /// <summary>
        /// 得到URL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string Url(this HttpRequest request)
        {
            return (request.Path.HasValue ? request.Path.Value : "") + request.UrlQuery();
        }
    }
}
