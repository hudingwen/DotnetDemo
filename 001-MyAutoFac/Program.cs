using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using MyAutoFac.Controllers;

namespace MyAutoFac
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);








            // 添加控制器服务
            builder.Services.AddControllers();



            //接管自带DI
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>((containerBuilder) =>
            {
                //ContainerBuilder containerBuilder = new ContainerBuilder();
                //构造函数注入
                containerBuilder.RegisterType<PropertyService>().As<IPropertyService>().PropertiesAutowired();
                //指定选择属性
                containerBuilder.RegisterType<UserService>().As<IUserService>().PropertiesAutowired(new CustomPropertySelector());
                //属性注入
                containerBuilder.RegisterType<TestController>().PropertiesAutowired();
                //接管控制器
                containerBuilder.RegisterType<AutofacControllerActivator>().As<IControllerActivator>().InstancePerLifetimeScope();

            });




            //构建 
            var app = builder.Build();
            //获取Autofac的IContainer容器
            ContainerHelper.container = app.Services.GetAutofacRoot();
             

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
