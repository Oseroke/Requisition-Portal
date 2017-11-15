using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace RequisitionPortal.BL.Infrastructure
{
    public sealed class NHibernateHelper
    {
        private const string CURRENT_NHIBERNATE_SESSION_KEY = "RequisitionPortal.NHIBERNATE.SESSION.KEY";
        private static readonly ISessionFactory sessionFactory;

        static NHibernateHelper()
        {
            sessionFactory = ConfigureNHibernate();
        }

        private static ISessionFactory ConfigureNHibernate()
        {
            var configure = new Configuration();
            var mapping = GetMappings();
            configure.Configure();
            configure.AddDeserializedMapping(mapping, "NHSchema");


            configure.Configure()
                     .SetProperty(NHibernate.Cfg.Environment.ConnectionStringName, "RequisitionPortalConnectionSetting")
                     .SetProperty(NHibernate.Cfg.Environment.ShowSql, "true")
                     .SetProperty(NHibernate.Cfg.Environment.BatchSize, "0");

            return configure.BuildSessionFactory();
        }
        private static void DefineBaseClass(ConventionModelMapper mapper, System.Type[] baseEntityToIgnore)
        {
            if (baseEntityToIgnore == null) return;
            mapper.IsEntity((type, declared) =>
                baseEntityToIgnore.Any(x => x.IsAssignableFrom(type)) &&
                !baseEntityToIgnore.Any(x => x == type) &&
                !type.IsInterface);
            mapper.IsRootEntity((type, declared) => baseEntityToIgnore.Any(x => x == type.BaseType));
        }

        private static HbmMapping GetMappings()
        {
            //var baseEntityToIgnore = new[] { 
            //    typeof(NHibernate.AspNet.Identity.DomainModel.EntityWithTypedId<int>), 
            //    typeof(NHibernate.AspNet.Identity.DomainModel.EntityWithTypedId<string>), 
            //};

            //var allEntities = new[] { 
            //    typeof(IdentityUser), 
            //    typeof(ApplicationUser), 
            //    typeof(IdentityRole), 
            //    typeof(IdentityUserLogin), 
            //    typeof(IdentityUserClaim), 
            //};

            var mapper = new ConventionModelMapper();
            mapper.AddMappings(Assembly.Load("RequisitionPortal.BL").GetTypes());

            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            
            System.Diagnostics.Debug.WriteLine(mapping.AsString());


            return mapping;

        }


        public static ISession GetCurrentSession()
        {
            HttpContext context = HttpContext.Current;
            ISession currentSession = context.Items[CURRENT_NHIBERNATE_SESSION_KEY] as ISession;

            if (currentSession == null)
            {
                currentSession = sessionFactory.OpenSession();
                context.Items[CURRENT_NHIBERNATE_SESSION_KEY] = currentSession;
            }
            if (currentSession.Connection.State == System.Data.ConnectionState.Closed)
            {
                currentSession = sessionFactory.OpenSession();

            }
            if (!currentSession.IsConnected)
            {
                currentSession = sessionFactory.OpenSession();

            }
            if (!currentSession.IsOpen)
            {
                currentSession = sessionFactory.OpenSession();

            }
            if (currentSession.IsDirty())
            {
                currentSession.Clear();

            }


            return currentSession;
        }

        public static void CloseSession()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                ISession currentSession = context.Items[CURRENT_NHIBERNATE_SESSION_KEY] as ISession;

                if (currentSession == null)
                {
                    // No current session
                    return;
                }
                currentSession.Clear();
                currentSession.Close();


                context.Items.Remove(CURRENT_NHIBERNATE_SESSION_KEY);
            }
        }

        public static void CloseSessionFactory()
        {
            if (sessionFactory != null)
            {
                sessionFactory.Close();
            }
        }
    }
}

