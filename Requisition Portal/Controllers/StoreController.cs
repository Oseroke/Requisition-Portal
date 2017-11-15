using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Requisition_Portal.Models;
using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Entities;
using RequisitionPortal.BL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Requisition_Portal.Controllers
{
    public class StoreController : BaseController
    {
        public IRequisitionService _reqService;
        public IStoreService _storeService;
        public ISetupService _setupService;

        public StoreController(IRequisitionService reqService, IStoreService storeService, ISetupService setupService)
        {
            _reqService = reqService;
            _storeService = storeService;
            _setupService = setupService;
        }

        // GET: Store
        public ActionResult Index()
        {
            //var model = new RequisitionModel();
            return View();
        }

        public ActionResult Req_Read([DataSourceRequest] DataSourceRequest request)
        {
            var requisitions = _reqService.GetOutstandingRequisitions((int)SystemEnums.Status.ManagerApproved);

            var _data = new List<RequisitionModel>();
            foreach (var req in requisitions)
            {
                var reqModel = new RequisitionModel()
                {
                    Id = req.Id,
                    Manager = req.Manager,
                    ReqDate = req.ReqDate,
                    Requestor = req.Requestor,
                    StatusID = req.StatusID,
                    //Status = ((SystemEnums.Status)(req.StatusID)).ToString(),
                    StatusDate = req.StatusDate,
                    Items = new List<ReqItemModel>()
                };

                foreach(var itm in req.Items)
                {
                    var reqItem = new ReqItemModel()
                    {
                        Id = itm.Id,
                        ChargeCode = itm.ChargeCode,
                        Description = itm.Description,
                        Quantity = itm.Quantity,
                        RequisitionID = itm.RequisitionID
                    };

                    reqModel.Items.Add(reqItem);
                }

                _data.Add(reqModel);
            }
            return Json(_data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Req_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<RequisitionModel> models)
        {
            var results = new List<RequisitionModel>();
            if (models != null && ModelState.IsValid)
            {
                foreach (var reqModel in models)
                {
                    try
                    {
                        var req = _reqService.GetRequisition(Convert.ToInt32(reqModel.Id));
                        req.StatusID = (int)SystemEnums.Status.AwaitingAcknowledgement;

                        reqModel.Status = ((SystemEnums.Status)req.StatusID).ToString();
                        results.Add(reqModel);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("update", ex.Message);
                        break;
                    }


                }
            }

            return Json(results.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateRequisition(int id)
        {
            try
            {
                var requisition = _reqService.GetRequisition(id);
                requisition.StatusID = (int)SystemEnums.Status.AwaitingAcknowledgement;
                _reqService.SaveRequisition(requisition);

                return RedirectToAction("Index");
            }

            catch
            {

                return View();
            }
        }

        public ActionResult ReadReqItems(long requisitionID, [DataSourceRequest] DataSourceRequest request)
        {
            var items = _reqService.GetRequisitionItems(requisitionID);

            var _data = new List<ReqItemModel>();
            
            foreach(var item in items)
            {
                _data.Add(new ReqItemModel()
                {
                    Id = item.Id,
                    Item = item.Item,
                    ChargeCode = item.ChargeCode,
                    Quantity = item.Quantity,
                    RequisitionID = item.RequisitionID
                });
            }
            

            return Json(_data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}