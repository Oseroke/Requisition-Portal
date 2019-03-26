using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using RequisitionPortal.Models;
using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RequisitionPortal.Controllers
{
    public class AuditController : BaseController
    {
        public IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public AuditController()
        {

        }
        // GET: Audit
        public ActionResult Trail()
        {
            return View();
        }

        public ActionResult Audit_Read([DataSourceRequest] DataSourceRequest request)
        {
            var auditTrails = _auditService.GetAuditTrails(-1);

            var _data = new List<AuditModel>();

            foreach(var audit in auditTrails)
            {
                var model = new AuditModel();
                model.Id = audit.Id;
                model.TimeStamp = audit.TimeStamp.ToShortDateString();
                model.Details = audit.Details;
                model.AuditAction = ((SystemEnums.AuditAction)audit.AuditActionId).ToString();

                _data.Add(model);
            }

            return Json(_data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
    }
}