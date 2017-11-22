using Autofac;
using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using Autofac.Integration.Mvc;
using RequisitionPortal.BL.Fake;
using RequisitionPortal.BL.Logic;

namespace Requisition_Portal.Infrastructure
{
    public class DependencyResolver
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            //Register All Controllers
            builder.RegisterControllers(Assembly.GetExecutingAssembly());


            //or be explicit
            //HTTP context and other related stuff
            //HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>().InstancePerHttpRequest();

            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerHttpRequest();


            //builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().InstancePerHttpRequest();

            builder.RegisterGeneric(typeof(NHibernateRepository<,>)).As(typeof(IRepository<,>)).InstancePerLifetimeScope();

            builder.RegisterType<RequisitionService>().As<IRequisitionService>().InstancePerHttpRequest();
            builder.RegisterType<StoreService>().As<IStoreService>().InstancePerHttpRequest();
            builder.RegisterType<SetupService>().As<ISetupService>().InstancePerHttpRequest();
            // builder.RegisterType<AutoMapperStartupTask>().Named<IStartupTask>("AutoMapper").InstancePerDependency();

            //builder.RegisterType<AsyncService>().As<IAsyncRunner>().InstancePerLifetimeScope();

            //builder.RegisterType<SetupService>().As<ISetupService>().InstancePerHttpRequest();
            //builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerLifetimeScope();
            //builder.RegisterType<FileService>().As<IFileService>().InstancePerLifetimeScope();
            //builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().InstancePerHttpRequest();

            //builder.RegisterType<UserService>().As<IUserService>().InstancePerHttpRequest();
            //builder.RegisterType<WebWorker>().As<IWebWorker>().InstancePerHttpRequest();
            //builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerHttpRequest();

            //builder.RegisterType<EmailAccount>().As<IEmailAccount>().InstancePerHttpRequest();
            //builder.RegisterType<UtilityService>().As<IUtilityService>().InstancePerHttpRequest();

            builder.RegisterType<AuditService>().As<IAuditService>().InstancePerHttpRequest();



            Autofac.IContainer container = builder.Build();
            System.Web.Mvc.DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}