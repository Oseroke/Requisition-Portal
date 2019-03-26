using RequisitionPortal.BL.Abstracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RequisitionPortal.Controllers
{
    public class HomeController : BaseController
    {

        public ActionResult Index()
        {
            //bool x = User.IsInRole("Admin");
            //var a = User.Identity.Name;
            //var y = Roles.FindUsersInRole("Admin", "OIgwubor");
            //var z = Roles.GetRolesForUser("OIgwubor");
            //Roles.CreateRole("SuperAdmin");
            //Roles.AddUserToRole("EOide", "Admin");   
            //Roles.AddUserToRole("OIgwubor", "Admin");

            return View();
        }

        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}