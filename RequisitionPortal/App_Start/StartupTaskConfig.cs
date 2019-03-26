using RequisitionPortal.Infrastructure;
using RequisitionPortal.BL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RequisitionPortal.App_Start
{
    public class StartupTaskConfig
    {

        public static void Register()
        {

            DependencyResolver.RegisterDependencies();



            //var autoMapper = EngineContext.Resolve<IStartupTask>("AutoMapper");
            //autoMapper.Execute();
        }
    }
}