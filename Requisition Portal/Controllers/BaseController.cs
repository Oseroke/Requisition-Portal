using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Requisition_Portal.Infrastructure;
using System.Text.RegularExpressions;

namespace Requisition_Portal.Controllers
{
    public class BaseController : Controller
    {
        //public ActionResult DownloadView()
        //{
        //    return this.ViewPdf("TitleMe", "Discos", "SetupController", null);
        //}
        public string DateFormat
        {
            get
            {
                return "dd/MM/yyyy";
            }
        }

        public int GridSize
        {
            get
            {
                return 10;
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {


            base.Initialize(requestContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //validate IP address
            //do we need to do any IP Validation here . . .??

            base.OnActionExecuting(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            //if (filterContext.Exception != null)
            //    LogException(filterContext.Exception);
            base.OnException(filterContext);
            var typeofException = filterContext.Exception.GetType();
            if (typeofException == typeof(ArgumentNullException))
            {
                //FormsAuthentication.SignOut();
            }
            else
                RedirectToAction("Error", "Error");

        }


        protected ActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl });
        }

        public static bool IsValidEmail(string email)
        {
            bool result = false;
            if (String.IsNullOrEmpty(email))
                return result;
            email = email.Trim();
            result = Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            return result;
        }

        protected virtual void ErrorNotification(Exception exception, bool persistForTheNextRequest = true, bool logException = true)
        {

            AddNotification(NotifyType.Error, exception.Message, persistForTheNextRequest);
        }

        protected virtual void AddNotification(NotifyType type, string message, bool persistForTheNextRequest)
        {
            string dataKey = string.Format("RequisitionPortal.notifications.{0}", type);
            if (persistForTheNextRequest)
            {
                if (TempData[dataKey] == null)
                    TempData[dataKey] = new List<string>();
                ((List<string>)TempData[dataKey]).Add(message);
            }
            else
            {
                if (ViewData[dataKey] == null)
                    ViewData[dataKey] = new List<string>();
                ((List<string>)ViewData[dataKey]).Add(message);
            }
        }

        protected virtual void SuccessNotification(string message, bool persistForTheNextRequest = true)
        {
            ShowJavascriptMessage(message);
            AddNotification(NotifyType.Success, message, persistForTheNextRequest);

        }


        protected virtual void ShowJavascriptMessage(string message)
        {
            //  var _tstshsh = "<script type='text/javascript' language='javascript' > alert('"  + "rrrr" + "');</script>";

            var _text = "<script type='text/javascript'>" + "alert('" + message + "');" + "</script>";
            string _Key = "RequisitionPortal.JavaScript";
            TempData[_Key] = _text;
            ViewData[_Key] = _text;
        }

    }
}