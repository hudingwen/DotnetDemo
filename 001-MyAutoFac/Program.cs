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








            // ��ӿ���������
            builder.Services.AddControllers();



            //�ӹ��Դ�DI
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>((containerBuilder) =>
            {
                //ContainerBuilder containerBuilder = new ContainerBuilder();
                //���캯��ע��
                containerBuilder.RegisterType<PropertyService>().As<IPropertyService>().PropertiesAutowired();
                //ָ��ѡ������
                containerBuilder.RegisterType<UserService>().As<IUserService>().PropertiesAutowired(new CustomPropertySelector());
                //����ע��
                containerBuilder.RegisterType<TestController>().PropertiesAutowired();
                //�ӹܿ�����
                containerBuilder.RegisterType<AutofacControllerActivator>().As<IControllerActivator>().InstancePerLifetimeScope();

            });




            //���� 
            var app = builder.Build();
            //��ȡAutofac��IContainer����
            ContainerHelper.container = app.Services.GetAutofacRoot();
             

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
