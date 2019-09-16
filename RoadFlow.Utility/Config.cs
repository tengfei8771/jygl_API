using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RoadFlow.Utility
{
    /// <summary>
    /// 系统配置类
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 当前系统版本
        /// </summary>
        public static string SystemVersion { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static string DatabaseType { get; set; }
        /// <summary>
        /// SqlServer连接字符串
        /// </summary>
        public static string ConnectionString_SqlServer { get; set; }
        /// <summary>
        /// MySql连接字符串
        /// </summary>
        public static string ConnectionString_MySql { get; set; }
        /// <summary>
        /// Oracle连接字符串
        /// </summary>
        public static string ConnectionString_Oracle { get; set; }
        /// <summary>
        /// 当前系统使用的连接字符串
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                switch (DatabaseType)
                {
                    case "sqlserver":
                        return ConnectionString_SqlServer;
                    case "mysql":
                        return ConnectionString_MySql;
                    case "oracle":
                        return ConnectionString_Oracle;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// cookie名称
        /// </summary>
        public static string CookieName { get; set; } = "RoadFlowCore.Session";

        /// <summary>
        /// 用户ID的Session Key
        /// </summary>
        public static string UserIdSessionKey { get; set; } = "RoadFlowUserId";

        /// <summary>
        /// session过期时间 
        /// </summary>
        public static int SessionTimeout { get; set; } = 20;

        /// <summary>
        /// 系统登录地址
        /// </summary>
        public static string LoginUrl { get; set; } = "/Home/Login";
        /// <summary>
        /// 人员初始密码
        /// </summary>
        public static string InitUserPassword { get; set; } = "111";
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public static int PageSize { get; set; } = 15;
        /// <summary>
        /// 是否调试模式(开发模式)
        /// </summary>
        public static bool IsDebug { get; set; } = false;
        /// <summary>
        /// 是否开启单点登录（只能在一个地方登录）
        /// </summary>
        public static bool SingleLogin { get; set; } = true;
        /// <summary>
        /// 是否将错误信息显示到客户端(0不显示 1显示)
        /// </summary>
        public static int ShowError { get; set; } = 0;
        /// <summary>
        /// 调试模式时的用户ID
        /// </summary>
        public static string DebugUserId { get; set; }
        /// <summary>
        /// 附件保存路径
        /// </summary>
        public static string FilePath { get; set; }
        /// <summary>
        /// 允许上传的文件类型
        /// </summary>
        public static string UploadFileExtNames { get; set; }
        /// <summary>
        /// 是否启用动态步骤功能
        /// </summary>
        public static bool EnableDynamicStep { get; set; } = false;


        #region 企业微信相关配置
        /// <summary>
        /// 企业ID
        /// </summary>
        public static string Enterprise_WeiXin_AppId { get; set; }
        /// <summary>
        /// 外网地址
        /// </summary>
        public static string Enterprise_WeiXin_WebUrl { get; set; }
        /// <summary>
        /// 是否使用企业微信
        /// </summary>
        public static bool Enterprise_WeiXin_IsUse { get; set; }
        /// <summary>
        /// 是否要同步组织架构
        /// </summary>
        public static bool Enterprise_WeiXin_IsSyncOrg { get; set; }
        #endregion

        #region 微信公众号相关配置
        /// <summary>
        /// 是否启用公众号
        /// </summary>
        public static bool WeiXin_IsUse { get; set; }
        /// <summary>
        /// 公众号APPID
        /// </summary>
        public static string WeiXin_AppId { get; set; }
        /// <summary>
        /// 公众号AppSecret
        /// </summary>
        public static string WeiXin_AppSecret { get; set; }
        /// <summary>
        /// 外网地址
        /// </summary>
        public static string WeiXin_WebUrl { get; set; }
        #endregion

        #region 引擎中心相关
        /// <summary>
        /// 是否启用引擎中心
        /// </summary>
        public static bool EngineCenter_IsUse { get; set; }
        #endregion

        #region 多语言配置
        /// <summary>
        /// 默认语言
        /// </summary>
        public static string Language_Default { get; set; }
        /// <summary>
        /// 语言列表
        /// </summary>
        public static List<CultureInfo> Language_CultureInfos
        {
            get
            {
                return new List<CultureInfo>{
                    new CultureInfo("zh-CN"),
                    new CultureInfo("zh"),
                    new CultureInfo("en-US")
                };
               
            }
        }
        /// <summary>
        /// 语言列表
        /// </summary>
        public static Dictionary<string, string> Language_Dictionary
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "zh-CN", "简体中文"},
                    { "zh", "繁體中文"},
                    { "en-US", "English"}
                };
            }
        }
        #endregion
    }
}
