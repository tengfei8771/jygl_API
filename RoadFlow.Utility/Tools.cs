using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Text;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;

namespace RoadFlow.Utility
{
    public class Tools
    {
        private static IHttpContextAccessor _accessor;
        private static IHostingEnvironment _hostingEnvironment;
        public static HttpContext HttpContext
        {
            get
            {
                return _accessor.HttpContext;
            }
        }

        public static void ConfigureHttpContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public static void ConfigureHostingEnvironment(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 验证实体
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static string GetValidateErrorMessag(ModelStateDictionary modelState)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int i = 1;
            stringBuilder.Append("验证错误：\n");
            foreach (string key in modelState.Keys)
            {
                var state = modelState[key];
                if (state.Errors.Any())
                {
                    stringBuilder.Append(i++);
                    stringBuilder.Append("、");
                    stringBuilder.Append(state.Errors.First().ErrorMessage);
                    stringBuilder.Append("\n");
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 得到当前请求的URL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetURL(HttpRequest request = null)
        {
            if (null == request)
            {
                var context = HttpContext;
                request = null != context ? context.Request : null;
            }
            if (null == request)
            {
                return string.Empty;
            }
            return new StringBuilder()
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }

        /// <summary>
        /// 得到当前请求的绝对URL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetAbsoluteURL(HttpRequest request = null)
        {
            if (null == request)
            {
                var context = HttpContext;
                request = null != context ? context.Request : null;
            }
            if (null == request)
            {
                return string.Empty;
            }
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }

        /// <summary>
        /// 得到当前请求主机
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetHttpHost(HttpRequest request = null)
        {
            if (null == request)
            {
                var context = HttpContext;
                request = null != context ? context.Request : null;
            }
            if (null == request)
            {
                return string.Empty;
            }
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .ToString();
        }


        /// <summary>
        /// 判断是否是ajax请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsAjax(HttpRequest request = null)
        {
            if (null == request)
            {
                var context = HttpContext;
                request = null != context ? context.Request : null;
            }
            if (null == request)
            {
                return false;
            }
            bool result = false;
            var xreq = request.Headers.ContainsKey("x-requested-with");
            if (xreq)
            {
                result = request.Headers["x-requested-with"] == "XMLHttpRequest";
            }
            return result;
        }

        /// <summary>
        /// 得到访问IP
        /// </summary>
        public static string GetIP()
        {
            var context = HttpContext;
            if (null == context)
            {
                return string.Empty;
            }
            return context.Connection.RemoteIpAddress.ToString();
        }

        /// <summary>
        /// 得到访问Agent
        /// </summary>
        /// <returns></returns>
        public static string GetBrowseAgent()
        {
            var context = HttpContext;
            if (null == context)
            {
                return string.Empty;
            }
            return context.Request.Headers["User-Agent"];
        }

        /// <summary>
        /// 得到上一个请求的URL地址
        /// </summary>
        /// <returns></returns>
        public static string GetReferer()
        {
            var context = HttpContext;
            if (null == context)
            {
                return string.Empty;
            }
            return context.Request.Headers["Referer"];
        }

        /// <summary>
        /// 产生随机数字
        /// </summary>
        /// <param name="start">开始数字</param>
        /// <param name="end">结束数字</param>
        /// <returns></returns>
        public static int GetRandomInt(int start, int end)
        {
            Random rd = new Random(Guid.NewGuid().ToInt()); ;
            return rd.Next(start, end);//(生成1~10之间的随机数，不包括10)
        }

        /// <summary>
        /// 产生随机字符串
        /// </summary>
        /// <returns>字符串位数</returns>
        public static string GetRandomString(int length)
        {
            int number;
            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random(Guid.NewGuid().ToInt());
            for (int i = 0; i < length; i++)
            {
                char code;
                number = random.Next();
                if (number % 2 == 0)
                    code = (char)('0' + (char)(number % 10));
                else
                    code = (char)('A' + (char)(number % 26));
                if (code.Equals('o') || code.Equals('0'))//去除O或者0
                {
                    length += 1;
                    continue;
                }
                stringBuilder.Append(code);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 得到分页大小
        /// </summary>
        /// <param name="setCookie">是否写入COOKIE</param>
        /// <returns></returns>
        public static int GetPageSize(bool setCookie = true)
        {
            string size = HttpContext.Request.Forms("pagesize");
            if (!size.IsInt())
            {
                size = HttpContext.Request.Querys("pagesize");
            }
            if (!size.IsInt())
            {
                if (HttpContext.Request.Cookies.TryGetValue("roadflowcorepagesize", out string s))
                {
                    size = s;
                }
            }
            int size2 = size.IsInt(out int size1) ? size1 : Config.PageSize;
            if (size2 <= 0)
            {
                size2 = Config.PageSize;
            }
            if (setCookie)
            {
                HttpContext.Response.Cookies.Append("roadflowcorepagesize", size2.ToString(), new CookieOptions() { Expires = DateExtensions.Now.AddYears(5) });
            }
            return size2;
        }

        /// <summary>
        /// 得到页号
        /// </summary>
        /// <returns></returns>
        public static int GetPageNumber()
        {
            string number = HttpContext.Request.Forms("pagenumber");
            if (!number.IsInt())
            {
                number = HttpContext.Request.Querys("pagenumber");
            }
            return number.IsInt(out int number1) ? number1 : 1;
        }

        /// <summary>
        /// 得到验证码图片
        /// </summary>
        /// <param name="code"></param>
        /// <param name="bgImg">背景图片，完整路径</param>
        /// <returns></returns>
        public static System.IO.MemoryStream GetValidateImg(out string code, string bgImg)
        {
            code = GetRandomString(4);
            Random rnd = new Random();
            System.Drawing.Bitmap img = new System.Drawing.Bitmap((int)Math.Ceiling((code.Length * 17.2)), 28);
            System.Drawing.Image bg = System.Drawing.Bitmap.FromFile(bgImg);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img);
            System.Drawing.Font font = new System.Drawing.Font("Arial", 16, (System.Drawing.FontStyle.Regular | System.Drawing.FontStyle.Italic));
            System.Drawing.Font fontbg = new System.Drawing.Font("Arial", 16, (System.Drawing.FontStyle.Regular | System.Drawing.FontStyle.Italic));
            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), System.Drawing.Color.Blue, System.Drawing.Color.DarkRed, 1.2f, true);
            g.DrawImage(bg, 0, 0, new System.Drawing.Rectangle(rnd.Next(bg.Width - img.Width), rnd.Next(bg.Height - img.Height), img.Width, img.Height), System.Drawing.GraphicsUnit.Pixel);
            g.DrawString(code, fontbg, System.Drawing.Brushes.White, 0, 1);
            g.DrawString(code, font, System.Drawing.Brushes.ForestGreen, 0, 1);//字颜色

            //画图片的背景噪音线 
            int x = img.Width;
            int y1 = rnd.Next(5, img.Height);
            int y2 = rnd.Next(5, img.Height);
            g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Green, 2), 1, y1, x - 2, y2);
            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Transparent), 0, 10, img.Width - 1, img.Height - 1);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms;
        }

        /// <summary>
        /// 判断当前是否是手机访问
        /// </summary>
        /// <returns></returns>
        public static bool IsPhoneAccess(HttpRequest request)
        {
            if (null == request)
            {
                var context = HttpContext;
                if (null != context)
                {
                    request = context.Request;
                }
            }
            if (null == request)
            {
                return false;
            }
            string u = request.Headers["User-Agent"];
            Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex w = new Regex(@".*wechat.*(\r\n)?", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return (b.IsMatch(u) || v.IsMatch(u.Substring(0, 4)) || w.IsMatch(u));
        }

        /// <summary>
        /// 判断字符串表达式
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static object ExecuteExpression(string Expression)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            return dt.Compute(Expression, "");
        }

        /// <summary>
        /// 得到web目录绝对路径(包含wwwroot)
        /// </summary>
        /// <returns></returns>
        public static string GetWebRootPath()
        {
            return null == _hostingEnvironment ? string.Empty : _hostingEnvironment.WebRootPath;
        }
        /// <summary>
        /// 得到站点目录绝对路径
        /// </summary>
        /// <returns></returns>
        public static string GetContentRootPath()
        {
            return null == _hostingEnvironment ? string.Empty : _hostingEnvironment.ContentRootPath;
        }
        
        /// <summary>
        /// 得到当前主题
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentTheme()
        {
            var httpContext = HttpContext;
            if (null == httpContext)
            {
                return "blue";
            }
            return httpContext.Request.Cookies.TryGetValue("rf_core_theme", out string theme) ? theme : "blue";
        }

        /// <summary>
        /// 执行反射
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns>object 执行结果，Exception 如果执行发生错误时的错误信息，为空表示成功</returns>
        public static (object, Exception) ExecuteMethod(string method, params object[] args)
        {
            Assembly assembly = GetAssembly(method, out string dllName);
            if (null == assembly)
            {
                return (null, new Exception("未能载入资源"));
            }
            try
            {
                string typeName = method.Substring(0, method.LastIndexOf('.'));
                string methodName = method.Substring(method.LastIndexOf('.') + 1);
                Type type = assembly.GetType(typeName, true);
                object instance = Activator.CreateInstance(type, false);
                return (type.GetMethod(methodName).Invoke(instance, args), null);
            }
            catch(Exception err)
            {
                return (null, err);
            }
        }

        /// <summary>
        /// 通过字符反射加载DLL对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string name, out string dllName)
        {
            dllName = string.Empty;
            //检查缓存中是否有该对象
            string key = "assembly_" + name;
            var obj = Cache.IO.Get(key);
            if (null != obj)
            {
                return (Assembly)obj;
            }
            
            if (name.IsNullOrWhiteSpace())
            {
                return null;
            }
            string[] names = name.Split('.');
            StringBuilder stringBuilder = new StringBuilder();
            Assembly assembly = null;
            foreach (string n in names)
            {
                try
                {
                    stringBuilder.Append(n);
                    stringBuilder.Append(".");
                    dllName = stringBuilder.ToString().TrimEnd('.');
                    assembly = Assembly.Load(dllName);
                    if (null != assembly)
                    {
                        break;
                    }
                }
                catch
                {

                }
            }
            Cache.IO.Insert(key, assembly);
            return assembly;
        }

        #region 语言相关
        /// <summary>
        /// 得到当前语言
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentLanguage()
        {
            //如果默认语言选项为空，说明配置文件中没有设置语言（不是多语言版）直接返回简体中文
            if (Config.Language_Default.IsNullOrWhiteSpace())
            {
                return "zh-CN";
            }
            string language = GetCookieLanguage();
            return language.IsNullOrWhiteSpace() ? Config.Language_Default : language;
        }
        /// <summary>
        /// 从COOKIE得到当前语言
        /// </summary>
        /// <returns></returns>
        public static string GetCookieLanguage()
        {
            string language = string.Empty;
            if (HttpContext != null && HttpContext.Request.Cookies.TryGetValue(".AspNetCore.Culture", out string def))
            {
                string[] defs = def.Split('|');
                if (defs.Length > 0)
                {
                    language = defs[0].TrimStart('c', '=').Trim();
                }
            }
            return language;
        }
        /// <summary>
        /// 得到语言下拉选项
        /// </summary>
        /// <param name="defaultLanuage"></param>
        /// <returns></returns>
        public static string GetLanuageOptions(string defaultLanuage = "")
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(var lan in Config.Language_Dictionary)
            {
                stringBuilder.Append("<option value=\"" + lan.Key + "\"" + (defaultLanuage.Equals(lan.Key) ? " selected=\"selected\"" : "") + ">" + lan.Value + "</option>");
            }
            return stringBuilder.ToString();
        }
        #endregion

    }
}
