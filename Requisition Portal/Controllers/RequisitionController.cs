using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Requisition_Portal.Models;
using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Entities;
using RequisitionPortal.BL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.Security;

namespace Requisition_Portal.Controllers
{
    public class RequisitionController : BaseController
    {

        public IRequisitionService _reqService;
        public IStoreService _storeService;
        public ISetupService _setupService;
        public IAuditService _auditService;
        public IEmailService _emailService;
        public IStaffService _staffService;
        public List<ReqItemModel> Items;
        private UserPrincipal user = UserPrincipal.Current;
        

        public RequisitionController(IRequisitionService reqService, IStoreService storeService, ISetupService setupService, IAuditService auditService, IEmailService emailService, IStaffService staffService)
        {
            _reqService = reqService;
            _storeService = storeService;
            _setupService = setupService;
            _auditService = auditService;
            _emailService = emailService;
            _staffService = staffService;
            Items = new List<ReqItemModel>();
            //username = UserPrincipal.Current.EmployeeId;

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
            //var check = User.IsInRole("Manager");
            var model = new RequisitionModel();

            var items = _storeService.GetItems(false, -1);
            var chargeCodes = _setupService.GetChargeCodes(false);

            var staff = _staffService.GetStaffByEmpCode(false, user.EmployeeId);
            var managers = _staffService.GetManagers(false, staff.ServLineCode);
            var staffModel = new StaffModel();

            //model.Managers.Add (new SelectListItem() { Text = "--Select Manager--", Value = "-1", Selected = true});
            foreach (var m in managers)
            {
                model.Managers.Add(new SelectListItem() { Text = m.EmpName, Value = m.EmpLogin });
            }

            var itemList = new List<ItemModel>();
            
            foreach (var itm in items)
            {
                itemList.Add(new ItemModel() { Id = itm.Id, Name = itm.Name, Quantity = itm.Quantity, Code = itm.Code });
            }

            var chargeCodeList = new List<ChargeCodeModel>();

            chargeCodeList.Add(new ChargeCodeModel() { Id = -1, Code = user.EmployeeId });

            foreach (var cc in chargeCodes)
            {
                chargeCodeList.Add(new ChargeCodeModel() { Id = cc.Id, Code = cc.Code });
            }
          
            ViewData["Items"] = itemList;
            ViewData["DefaultItem"] = itemList.FirstOrDefault();
            
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
                //TempData["Message"] = "";

                int count = fc.Count / 3; //3 because we have 3 columns in the table
                string storeError = "";
                string message = null;

                if (count >= 1)
                {
                    
                    var requisition = new Requisition()
                    {
                        ManagerID = fc["Manager"],
                        //ManagerName = fc["Manager"],
                        ReqDate = DateTime.Today,
                        StatusDate = DateTime.Today,
                        UnitID = 1,
                        RequestorID = user.Name,
                        //RequestorName = UserPrincipal.Current.DisplayName,
                        StatusID = (int)SystemEnums.Status.AwaitingMgrApproval,
                        IsDeleted = false,
                        SentToAccounts = false,
                        PostedByAccounts = false,
                        Items = new List<Req_Item>()
                    };

                    if (requisition.ManagerID == "-1")
                    {
                        TempData["Message"] = "Select a manager";
                        return RedirectToAction("Create");
                    }
                    for (int i = 0; i < count; i++)
                    {
                        var reqItem = new Req_Item()
                        {
                            Item = fc["ReqItemModel[" + i + "].Item.Code"],
                            Quantity = Int32.Parse(fc["ReqItemModel[" + i + "].Quantity"]),
                            ChargeCode = fc["ReqItemModel[" + i + "].ChargeCode.Code"],
                            IsDeleted = false,
                            //ItemNo = 2  //This one still requires modification
                        };

                        var storeItem = _storeService.GetItem(reqItem.Item);
                        if (storeItem != null)
                        {
                            if (storeItem.Quantity > reqItem.Quantity)
                            {
                                requisition.Items.Add(reqItem);
                                
                            }
                            else if (storeItem.Quantity == 0)
                            {
                                message += storeItem.Name + " is not available in the store right now. " ;
                                //Log missing item
                            }
                            else if (storeItem.Quantity < reqItem.Quantity)
                            {
                                message += "The quantity of " + storeItem.Name + " that you ordered for is less than what is available in the store right now. " + "";
                            }
                        }
                    }

                    if (requisition.Items.Count > 0)
                    {
                        _reqService.SaveRequisition(requisition);
                        

                        foreach (var item in requisition.Items)
                        {
                            var storeItem = _storeService.GetItem(item.Item);
                            storeItem.Quantity -= item.Quantity;
                            try
                            {
                                _storeService.SaveItem(storeItem);
                            }
                            catch
                            {
                                //Log issue somewhere stating that the store record did not update
                            }
                        }

                        //Log activity
                        try
                        {
                            _auditService.LogRequisitionActivity(requisition, SystemEnums.AuditAction.MadeRequisition);
                        }
                        catch { }

                        //Send an email
                        try
                        {
                            //_emailService.SendEmail("oseroke.igwubor@ng.kpmg.com", "New Requisition", "Test Test Test");
                        }
                        catch { }
                        
                        TempData["Message"] = "Your requisition has been created Successfully ";
                        return RedirectToAction("History");
                    }
                    else
                    {
                        message += " No requisition has been made. Select at least one available item. ";
                        TempData["Message"] = message;
                        return RedirectToAction("Create");
                    }
                    
                }
                else
                {
                    message += " An error occured while making your requisition. Add at least one item.";
                    TempData["Message"] = message;
                    return RedirectToAction("Create");
                }                 

            }
            catch
            {
                TempData["Message"] = " An error has occured. Please try again or contact your administrator";
                return RedirectToAction("Create");
            }
        }

        public ActionResult Respond(int requisitionId)
        {
            //var username = User.Identity.Name;
            
            var requisition = _reqService.GetRequisition(requisitionId, user.Name);

            var manager = _staffService.GetStaffByUsername(false, requisition.ManagerID);
            var requestor = _staffService.GetStaffByUsername(false, requisition.RequestorID);

            var model = new RequisitionModel()
            {
                Id = requisition.Id,
                Manager = manager != null ? manager.EmpName : "",
                ReqDate = requisition.ReqDate,
                Requestor = requestor != null ? requestor.EmpName : "",
                StatusID = requisition.StatusID,
                //Status = ((SystemEnums.Status)requisition.StatusID).ToString(),
                StatusDate = requisition.StatusDate,
                UnitID = requisition.UnitID
            };

            foreach(var item in requisition.Items)
            {
                var itmModel = new ReqItemModel();

                itmModel.Id = item.Id;
                    //Item = item.Item,
                    //ChargeCode = item.ChargeCode,
                itmModel.Quantity = item.Quantity;
                itmModel.Item.Name = item.Item;
                itmModel.ChargeCode.Code = item.ChargeCode;

                model.Items.Add(itmModel);
                
            }

            return View(model);// Change this to a list of outstanding approvals;
        }

        [HttpPost]
        public ActionResult Respond(FormCollection fc)
        {
            string buttonClicked = Request.Form["SubmitButton"];

            int requisitionId = Int32.Parse(fc["Id"]);
            var requisition = _reqService.GetRequisition(requisitionId, user.Name);

            if (buttonClicked == "Approve")
            {
                requisition.StatusID = (int)SystemEnums.Status.ManagerApproved;
                requisition.StatusDate = DateTime.Now;
                requisition.ManagerID = user.Name;
                //requisition.ManagerName = UserPrincipal.Current.DisplayName;
            }
            else if(buttonClicked == "Reject")
            {
                requisition.StatusID = (int)SystemEnums.Status.ManagerCancelled;
                requisition.StatusDate = DateTime.Now;
                requisition.ManagerID = user.Name;
                //requisition.ManagerName = UserPrincipal.Current.DisplayName;                
                //return item to store                
            }

            _reqService.SaveRequisition(requisition);
            _auditService.LogRequisitionActivity(requisition, buttonClicked == "Approve" ? SystemEnums.AuditAction.ApprovedRequisition : SystemEnums.AuditAction.RejectedRequisition);
            //Log activity
            //Send Email

            return RedirectToAction("Create");
        }      

        public ActionResult Response()
        {
            return View();
        }

        public ActionResult History()
        {
            return View();
        }

        public ActionResult Req_Read([DataSourceRequest] DataSourceRequest request)
        {
            var requisitions = _reqService.GetRequisitions(-1, user.Name);

            var _data = new List<RequisitionModel>();
            foreach (var req in requisitions)
            {
                var manager = _staffService.GetStaffByUsername(false, req.ManagerID);
                var requestor = _staffService.GetStaffByUsername(false, req.RequestorID);
                var reqModel = new RequisitionModel()
                {
                    Id = req.Id,
                    Manager = manager != null ? manager.EmpName : "",// req.ManagerName,
                    ReqDate = req.ReqDate,
                    Requestor = requestor != null ? requestor.EmpName : "",
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
                        //ChargeCode = itm.ChargeCode,
                        Description = itm.Description,
                        Quantity = itm.Quantity,
                        RequisitionID = itm.RequisitionID
                    };
                    reqItem.ItemName = _storeService.GetItem(itm.Item).Name;
                    reqItem.ChargeCode.Code = itm.ChargeCode;
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
                        var req = _reqService.GetRequisition(Convert.ToInt32(reqModel.Id), user.Name);
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

        public ActionResult CancelRequisition(int id)
        {
            try
            {
                var requisition = _reqService.GetRequisition(id, user.Name);
                requisition.StatusID = (int)SystemEnums.Status.UserCancelled;
                requisition.StatusDate = DateTime.Now;
                _reqService.SaveRequisition(requisition);

                _auditService.LogRequisitionActivity(requisition, SystemEnums.AuditAction.CancelledRequisition);

                //return item to store
                foreach(var item in requisition.Items)
                {
                    var itm = _storeService.GetItem(item.Item);
                    itm.Quantity += item.Quantity;
                    _storeService.SaveItem(itm);
                }

                return RedirectToAction("Index");
            }

            catch
            {

                return View();
            }
        }

        public ActionResult CompleteRequisition(int id)
        {
            try
            {
                var requisition = _reqService.GetRequisition(id, user.Name);
                requisition.StatusID = (int)SystemEnums.Status.Completed;
                requisition.StatusDate = DateTime.Now;
                _reqService.SaveRequisition(requisition);

                _auditService.LogRequisitionActivity(requisition, SystemEnums.AuditAction.Completed);
                                
                return RedirectToAction("History");
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
                var itemModel = new ReqItemModel();
                                
                itemModel.Id = item.Id;
                itemModel.ItemName = _storeService.GetItem(item.Item).Name;
                itemModel.ChargeCode.Code = item.ChargeCode;
                itemModel.Quantity = item.Quantity;
                itemModel.RequisitionID = item.RequisitionID;
                
                _data.Add(itemModel);
            }


            return Json(_data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult OutstandingReqRead([DataSourceRequest] DataSourceRequest request)
        {
            var requisitions = _reqService.GetRequisitions((int)SystemEnums.Status.AwaitingMgrApproval, user.Name);

            var _data = new List<RequisitionModel>();
            foreach (var req in requisitions)
            {
                var manager = _staffService.GetStaffByUsername(false, req.ManagerID);
                var requestor = _staffService.GetStaffByUsername(false, req.RequestorID);
                var reqModel = new RequisitionModel()
                {
                    Id = req.Id,
                    Manager = manager != null ? manager.EmpName : "",// req.ManagerName,
                    ReqDate = req.ReqDate,
                    Requestor = requestor != null ? requestor.EmpName : "",
                    StatusID = req.StatusID,
                    //Status = ((SystemEnums.Status)(req.StatusID)).ToString(),
                    StatusDate = req.StatusDate,
                    Items = new List<ReqItemModel>()
                };

                foreach (var itm in req.Items)
                {
                    var itemModel = new ReqItemModel();

                    itemModel.Id = itm.Id;
                    itemModel.ChargeCode.Code = itm.ChargeCode;
                    itemModel.Description = itm.Description;
                    itemModel.Quantity = itm.Quantity;
                    itemModel.RequisitionID = itm.RequisitionID;
                    itemModel.ItemName = _storeService.GetItem(itm.Item).Name;

                    reqModel.Items.Add(itemModel);
                }

                _data.Add(reqModel);
            }
            return Json(_data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult OutstandingReadReqItems(long requisitionID, [DataSourceRequest] DataSourceRequest request)
        {
            var items = _reqService.GetRequisitionItems(requisitionID);

            var _data = new List<ReqItemModel>();

            foreach (var item in items)
            {
                var itemModel = new ReqItemModel();


                itemModel.Id = item.Id;
                itemModel.ItemName = _storeService.GetItem(item.Item).Name;
                itemModel.ChargeCode.Code = item.ChargeCode;
                itemModel.Quantity = item.Quantity;
                itemModel.RequisitionID = item.RequisitionID;                

                _data.Add(itemModel);
            }


            return Json(_data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApproveRequisition(int id)
        {
            try
            {
                var requisition = _reqService.GetRequisition(id, user.Name);
                requisition.StatusID = (int)SystemEnums.Status.ManagerApproved;
                requisition.StatusDate = DateTime.Now;
                //requisition.ManagerID = user.Name;
                _reqService.SaveRequisition(requisition);

                try
                {
                    _auditService.LogRequisitionActivity(requisition, SystemEnums.AuditAction.ApprovedRequisition);
                }
                catch
                {

                }

                return RedirectToAction("Response");
            }

            catch
            {
                return View();
            }
        }

        public ActionResult RejectRequisition(int id)
        {
            try
            {
                var requisition = _reqService.GetRequisition(id,user.Name);
                requisition.StatusID = (int)SystemEnums.Status.ManagerCancelled;
                requisition.StatusDate = DateTime.Now;
                //requisition.ManagerID = user.Name
                _reqService.SaveRequisition(requisition);

                try
                {
                    _auditService.LogRequisitionActivity(requisition, SystemEnums.AuditAction.RejectedRequisition);
                }
                catch
                {

                }
                return RedirectToAction("Response");
            }

            catch
            {
                return View();
            }
        }

        public ActionResult FilterMenuCustomization_Requestor()
        {
            var db = _reqService.GetRequisitions(-1, user.Name);
            //var model = new RequisitionModel();

            //foreach(var min model )
            return Json(db.Select(e => e.RequestorID).Distinct(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterMenuCustomization_Status()
        {
            var db = _reqService.GetRequisitions(-1, user.Name);
            var models = new List<RequisitionModel>();

            foreach (var req in db)
            {
                var manager = _staffService.GetStaffByUsername(false, req.ManagerID);
                var requestor = _staffService.GetStaffByUsername(false, req.RequestorID);
                models.Add(new RequisitionModel()
                {
                    Id = req.Id,
                    Manager = manager != null ? manager.EmpName : "",
                    ReqDate = req.ReqDate,
                    Requestor = requestor != null ? requestor.EmpName : "",
                    StatusID = req.StatusID,
                    //Status = ((SystemEnums.Status)(req.StatusID)).ToString(),
                    StatusDate = req.StatusDate
                });
            }
            return Json(models.Select(e => e.StatusString).Distinct(), JsonRequestBehavior.AllowGet);
        }

    }
}
