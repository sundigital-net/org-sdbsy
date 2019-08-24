using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Reflection;
using Autofac;
using log4net;
using Autofac.Integration.Mvc;
using SDBSY.Common;
using SDBSY.IService;
using SDBSY.UserWebNew.App_Start;

namespace SDBSY.UserWebNew
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static ILog log = LogManager.GetLogger(typeof(MvcApplication));
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            var builder=new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly)
                .PropertiesAutowired(); //把当前程序集中的Controller都注册        
            //不要忘了.PropertiesAutowired(),属性注入必须的，下面的也是
            // 获取所有相关类库的程序集
            Assembly[] assemblies = new Assembly[] { Assembly.Load("SDBSY.Service") };
            builder.RegisterAssemblyTypes(assemblies)
                .Where(type => !type.IsAbstract
                               && typeof(IServiceSupport).IsAssignableFrom(type))
                .AsImplementedInterfaces()
                .InstancePerRequest()//单个 Web / HTTP / API 请求的范围内的组件的共享一个实例。仅可用于支持每个请求的依赖关系的整合（如MVC，Web API，Web Forms等）。
                .PropertiesAutowired();
            var container = builder.Build();
            //注册系统级别的DependencyResolver，这样当MVC框架创建Controller等对象的时候都是管Autofac要对象。
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container)); //!!!


            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalFilters.Filters.Add(new SDBSYExceptionFilter());//异常记录
            GlobalFilters.Filters.Add(new SDBSYAuthorizeFilter());//权限检查
            GlobalFilters.Filters.Add(new JsonNetActionFilter());//json序列化
        }
    }
}
