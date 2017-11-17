using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
//using Microsoft.AspNetCore.Mvc;
using Requisition_Portal.Helpers;
using Requisition_Portal.Models;
using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Entities;
using RequisitionPortal.BL.Infrastructure;
using RequisitionPortal.BL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Requisition_Portal.Controllers
{
    public class RequisitionController : BaseController
    {

        public IRequisitionService _reqService;
        public IStoreService _storeService;
        public ISetupService _setupService;
        public List<ReqItemModel> Items;

        public RequisitionController(IRequisitionService reqService, IStoreService storeService, ISetupService setupService)
        {
            _reqService = reqService;
            _storeService = storeService;
            _setupService = setupService;
            Items = new List<ReqItemModel>();

        }
        // GET: Requisition
        public ActionResult Index()
        {
            return View();
        }

        // GET: Requisition/Details/5
        public ActionResult Details(int id=0)
        {
            return View();
        }

        // GET: Requisition/Create
        public ActionResult Create()
        {
            var model = new RequisitionModel();

            var items = _storeService.GetItems(false);
            var chargeCodes = _setupService.GetChargeCodes(false);

            var itemList = new List<ItemModel>();
            
            foreach (var itm in items)
            {
                itemList.Add(new ItemModel() { Id = itm.Id, Name = itm.Name, Quantity = itm.Quantity });
            }

            var chargeCodeList = new List<ChargeCodeModel>();

            foreach(var cc in chargeCodes)
            {
                chargeCodeList.Add(new ChargeCodeModel() { Id = cc.Id, Code = cc.Code });
            }
          
            ViewData["Items"] = itemList;
            
            ViewData["ChargeCodes"] = chargeCodeList;
            ViewData["DefaultChargeCode"] = chargeCodeList.FirstOrDefault();
            
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Item_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ReqItemModel> itemModels)
        {
            var items = Items;// new List<ReqItemModel>();
            if (itemModels != null && ModelState.IsValid)
            {
                foreach (var item in itemModels)
                {
                    try
                    {
                        items.Add(item);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("create", ex.Message);
                        break;
                    }
                }
            }

            return Json(items.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        public ActionResult Item_Read([DataSourceRequest] DataSourceRequest request)
        {
            var _data = Items;            

            return Json(_data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // POST: Requisition/Create
        [HttpPost]
        public ActionResult Create(RequisitionModel model, FormCollection fc)
        {
            try
            {
                // TODO: Add insert logic here

                int count = fc.Count / 3; //3 because we have 3 columns in the table

                if (count > 1)
                {

                    var requisition = new Requisition()
                    {
                        Manager = fc["Manager"],
                        ReqDate = DateTime.Today,
                        StatusDate = DateTime.Today,
                        UnitID = 1,
                        Requestor = "150053942",
                        StatusID = (int)SystemEnums.Status.AwaitingMgrApproval,
                        IsDeleted = false,
                        Items = new List<Req_Item>()
                    };

                    for (int i = 0; i < count; i++)
                    {
                        requisition.Items.Add(new Req_Item()
                        {
                            Item = fc["ReqItemModel[" + i + "].Item"],
                            Quantity = Int32.Parse(fc["ReqItemModel[" + i + "].Quantity"]),
                            ChargeCode = fc["ReqItemModel[" + i + "].ChargeCode"],
                            IsDeleted = false,
                            ItemNo = 1  //This one still requires modification
                        });
                    }
                    _reqService.SaveRequisition(requisition);
                    //Log the activity
                    //Send an email

                }
                else
                {
                    ErrorNotification(new Exception(),true,false);
                    ShowJavascriptMessage("error");
                    //ShowJavascriptMessage("Select at least one item");
                }

                TempData["Message"] = "Your requisition has been created Successfully ";

                return RedirectToAction("Create");

            }
            catch
            {
                return View();
            }
        }

        public ActionResult Respond(int requisitionId)
        {
            var requisition = _reqService.GetRequisition(requisitionId);

            var model = new RequisitionModel()
            {
                Id = requisition.Id,
                Manager = requisition.Manager,
                ReqDate = requisition.ReqDate,
                Requestor = requisition.Requestor,
                StatusID = requisition.StatusID,
                //Status = ((SystemEnums.Status)requisition.StatusID).ToString(),
                StatusDate = requisition.StatusDate,
                UnitID = requisition.UnitID
            };

            foreach(var item in requisition.Items)
            {
                model.Items.Add(new ReqItemModel()
                {
                    Id = item.Id,
                    Item = item.Item,
                    ChargeCode = item.ChargeCode,
                    Quantity = item.Quantity
                });
            }

            return View(model);// Change this to a list of outstanding approvals;
        }

        [HttpPost]
        public ActionResult Respond(FormCollection fc)
        {
            string buttonClicked = Request.Form["SubmitButton"];

            int requisitionId = Int32.Parse(fc["Id"]);
            var requisition = _reqService.GetRequisition(requisitionId);

            if (buttonClicked == "Approve")
            {
                requisition.StatusID = (int)SystemEnums.Status.ManagerApproved;
                requisition.StatusDate = DateTime.Now;
            }
            else if(buttonClicked == "Reject")
            {
                requisition.StatusID = (int)SystemEnums.Status.ManagerCancelled;
                requisition.StatusDate = DateTime.Now;
            }

            _reqService.SaveRequisition(requisition);
            //Log activity
            //Send Email

            return RedirectToAction("Create");
        }      

        public ActionResult History()
        {
            return View();
        }

        public ActionResult Req_Read([DataSourceRequest] DataSourceRequest request)
        {
            var requisitions = _reqService.GetRequisitions(-1);

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

                foreach (var itm in req.Items)
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

        //[HttpPost]
        public ActionResult CancelRequisition(int id)
        {
            try
            {
                var requisition = _reqService.GetRequisition(id);
                requisition.StatusID = (int)SystemEnums.Status.UserCancelled;
                requisition.StatusDate = DateTime.Now;
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

            foreach (var item in items)
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
