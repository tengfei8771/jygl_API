using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UIDP.WebAPI
{
    /// <summary>
    /// 使用Current.HttpContext 扩展类
    /// </summary>
    public static class StaticHttpContextExtensions
    {
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            var hostingEnvironment = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            //var hubContext = app.ApplicationServices.GetRequiredService<IHubContext<Business.SignalR.SignalRHub>>();
            RoadFlow.Utility.Tools.ConfigureHttpContext(httpContextAccessor);//注册HttpContext,为了在任意地方得到HttpContext
            RoadFlow.Utility.Tools.ConfigureHostingEnvironment(hostingEnvironment);//注册hostingEnvironment，为了在任意地方得到当前程序路径
            //Business.SignalR.SignalRHub.Configure(hubContext);//注册SignalRHub,在任意地方向客户端发送消息
            return app;
        }
    }
}
