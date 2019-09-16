using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using UEditor.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RoadFlow.Utility;
using Microsoft.AspNetCore.Http.Features;

namespace UIDP.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            RoadFlow.Utility.Config.InitUserPassword = Configuration.GetSection("InitUserPassword").Value;
            RoadFlow.Utility.Config.IsDebug = "1".Equals(Configuration.GetSection("IsDebug").Value);
            RoadFlow.Utility.Config.DebugUserId = Configuration.GetSection("DebugUserId").Value;
            RoadFlow.Utility.Config.SingleLogin = "1".Equals(Configuration.GetSection("SingleLogin").Value);
            RoadFlow.Utility.Config.ShowError = Configuration.GetSection("ShowError").Value.ToInt(0);
            RoadFlow.Utility.Config.FilePath = Configuration.GetSection("FilePath").Value.TrimEnd('/').TrimEnd('\\');
            RoadFlow.Utility.Config.UploadFileExtNames = Configuration.GetSection("FileExtName").Value;
            RoadFlow.Utility.Config.DatabaseType = Configuration.GetSection("DatabaseType").Value.ToLower();
            RoadFlow.Utility.Config.ConnectionString_SqlServer = Configuration.GetConnectionString("RF_SqlServer");
            RoadFlow.Utility.Config.ConnectionString_MySql = Configuration.GetConnectionString("RF_MySql");
            RoadFlow.Utility.Config.ConnectionString_Oracle = Configuration.GetConnectionString("RF_Oracle");
            RoadFlow.Utility.Config.EnableDynamicStep = Configuration.GetSection("EnableDynamicStep").Value.ToInt(0) == 1;
            RoadFlow.Utility.Config.SystemVersion = Configuration.GetSection("Version").Value;
            var enterpriseWeiXin = Configuration.GetSection("EnterpriseWeiXin");
            if (null != enterpriseWeiXin)
            {
                RoadFlow.Utility.Config.Enterprise_WeiXin_AppId = enterpriseWeiXin.GetSection("AppId").Value;
                RoadFlow.Utility.Config.Enterprise_WeiXin_WebUrl = enterpriseWeiXin.GetSection("WebUrl").Value;
                RoadFlow.Utility.Config.Enterprise_WeiXin_IsUse = "1".Equals(enterpriseWeiXin.GetSection("IsUse").Value);
                RoadFlow.Utility.Config.Enterprise_WeiXin_IsSyncOrg = "1".Equals(enterpriseWeiXin.GetSection("IsSyncOrg").Value);
            }
            var weiXin = Configuration.GetSection("WeiXin");
            if (null != weiXin)
            {
                RoadFlow.Utility.Config.WeiXin_IsUse = "1".Equals(weiXin.GetSection("IsUse").Value);
                RoadFlow.Utility.Config.WeiXin_AppId = weiXin.GetSection("AppId").Value;
                RoadFlow.Utility.Config.WeiXin_AppSecret = weiXin.GetSection("AppSecret").Value;
                RoadFlow.Utility.Config.WeiXin_WebUrl = weiXin.GetSection("WebUrl").Value;
            }
            var EngineCenter = configuration.GetSection("EngineCenter");
            if (null != EngineCenter)
            {
                RoadFlow.Utility.Config.EngineCenter_IsUse = "1".Equals(EngineCenter.GetSection("IsUse").Value);
            }
            var sessionConfig = Configuration.GetSection("Session");
            if (null != sessionConfig)
            {
                RoadFlow.Utility.Config.UserIdSessionKey = sessionConfig.GetValue<string>("UserIdKey");
                RoadFlow.Utility.Config.CookieName = sessionConfig.GetValue<string>("CookieName");
                RoadFlow.Utility.Config.SessionTimeout = sessionConfig.GetValue<int>("TimeOut");
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("all", builder =>
                {
                    builder.AllowAnyOrigin() //允许任何来源的主机访问
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//指定处理cookie
                });
            });
            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); 
            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(x =>
            {

                x.ValueLengthLimit = int.MaxValue;

                x.MultipartBodyLengthLimit = int.MaxValue;
 
                x.MultipartHeadersLengthLimit = int.MaxValue;


            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                //This lambda determines whether user consent for non - essential cookies is needed for a given request.
                //options.CheckConsentNeeded = context => true;
                //options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //设置表单可提交内容长度
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
                options.KeyLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MultipartBoundaryLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });
            services.AddSession(options =>
            {
                options.Cookie.Name = RoadFlow.Utility.Config.CookieName;
                options.IdleTimeout = TimeSpan.FromMinutes(RoadFlow.Utility.Config.SessionTimeout);//设置session的过期时间
            });
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddTimedJob();
            services.AddMemoryCache();
            services.AddUEditorService("ueditor.json", true);
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ExcelModel")),
                RequestPath = "/ExcelModel"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
       System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ExcelModel/Templates")),
                RequestPath = "/ExcelModel/Templates"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Files/export")),
                RequestPath = "/Files/export"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
               System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "UploadFiles/img")),
                RequestPath = "/UploadFiles/img",
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=36000");
                }
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "UploadFiles/notice")),
                RequestPath = "/UploadFiles/notice",
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=36000");
                }
            });
            #region 解决Ubuntu Nginx 代理不能获取IP问题
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            });
            #endregion
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowAnyOrigin();
            });
            app.UseCors("all");
            app.UseTimedJob();
            app.UseStaticFiles();
            app.UseStaticHttpContext();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
