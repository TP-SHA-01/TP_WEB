using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using TB_WEB.CommonLibrary.Log;

namespace WebApi.App_Start
{
    public class AutofacConfig
    {
        private static IContainer _container;

        public static void ConfigureContainer()
        {
            #region AutoFac IOC容器

            var builder = new ContainerBuilder();

            try
            {
                //builder.RegisterControllers(Assembly.GetCallingAssembly());  //注册mvc控制器  需要引用package Autofac.Mvc

                //builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();  //支持Api控制器属性注入
                builder.RegisterApiControllers(Assembly.GetCallingAssembly());  //注册所有api控制器  构造函数注入  需要引用package Autofac.WebApi
                //注册程序集
                #region Service
                var assemblysServices = Assembly.Load("WebApi");
                builder.RegisterAssemblyTypes(assemblysServices)
                .AsImplementedInterfaces()
                .InstancePerDependency();
                #endregion

                builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
                _container = builder.Build();   //创建依赖注入


                //设置MVC依赖注入
                //DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));

                //设置WebApi依赖注入
                GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(_container);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }

            #endregion
        }

        /// <summary>
        /// 从Autofac容器获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetFromFac<T>()
        {
            return _container.Resolve<T>();
        }
    }
}