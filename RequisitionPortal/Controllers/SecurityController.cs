using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RequisitionPortal.Controllers
{
    public class SecurityController : BaseController
    {
        // GET: Security
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}