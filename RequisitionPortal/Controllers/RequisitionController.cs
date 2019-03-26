using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using RequisitionPortal.Models;
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
using System.Web.WebPages;
using System.Security.Principal;
using System.Web.Security;

namespace RequisitionPortal.Controllers
{
    public class RequisitionController : BaseController
    {
        public IRequisitionService _reqService;
        public IStoreService _storeService;
        public ISetupService _setupService;
        public IAuditService _auditService;
        public IEmailService _emailService;
        public IStaffService _staffService;
        public IErrorService _errorService;
        public List<ReqItemModel> Items;
        public WindowsIdentity user = WindowsIdentity.GetCurrent();        

        public RequisitionController(IRequisitionService reqService, IStoreService storeService, ISetupService setupService, IAuditService auditService, IEmailService emailService, IStaffService staffService, IErrorService errorService)
        {
            _reqService = reqService;
            _storeService = storeService;
            _setupService = setupService;
            _auditService = auditService;
            _emailService = emailService;
            _staffService = staffService;
            _errorService = errorService;
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
            //var check = User.IsInRole("Manager");
            var model = new RequisitionModel();

            var items = _storeService.GetItems(false, -1);

            if(user == null)
            {
                ViewData["Name"] = "null";
                return RedirectToAction("Index", "Home");
            }

            //var staff = _staffService.GetUserByUsername(false, user.Name.Remove(0,3));

            var managers = _staffService.GetManagersByDepartment(false, "");//staff.Department);


            var chargeableCodes = _setupService.GetChargeableChargeCodes(false);
            var nonChargeableCodes = _setupService.GetNonChargeableChargeCodes(false, "");// staff.Department);
            //var staffModel = new StaffModel();

            model.Managers.Add (new SelectListItem() { Text = "--Select Manager--", Value = "-1", Selected = true});
            foreach (var m in managers)
            {
                if (m.Username != user.Name.Remove(0,3))
                    model.Managers.Add(new SelectListItem() { Text = m.DisplayName, Value = m.Username });

            }
            var itemList = new List<ItemModel>();
            
            foreach (var itm in items)
            {
                itemList.Add(new ItemModel() { Id = itm.Id, Name = itm.Name, Quantity = itm.Quantity, Code = itm.Code });
            }

            var chargeCodeList = new List<ChargeCodeModel>();

            //make the employee the default first chargecode
            chargeCodeList.Add(new ChargeCodeModel() { Id = -1, Code = user.Name.Remove(0, 3) });//.EmployeeId });
            //chargeCodeList.Add(new ChargeCodeModel() { Id = -1, Code = staff.Username });//.EmployeeId });

            foreach (var cc in chargeableCodes)
            {
                chargeCodeList.Add(new ChargeCodeModel() { Id = cc.Id, Code = cc.Code });
            }

            foreach (var cc in nonChargeableCodes)
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

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
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
                string message = null;
                int unavailable = 0;

                if (count >= 1)
                {                  
                    var requisition = new Requisition()
                    {
                        ManagerID = fc["Manager"],
                        ReqDate = DateTime.Today,
                        StatusDate = DateTime.Today,
                        UnitID = 1,
                        RequestorID = user.Name.Remove(0,3),
                        StatusID = (int)SystemEnums.Status.AwaitingMgrApproval,
                        IsDeleted = false,
                        SentToAccounts = false,
                        PostedByAccounts = false,
                        Items = new List<Req_Item>()
                    };

                    if (requisition.ManagerID == "-1")
                    {
                        TempData["Message"] = "Select a manager";
                        //var newModel = new RequisitionModel();
                        //for (int i = 0; i < count; i++)
                        //{
                        //    var reqItem = new ReqItemModel();
                        //    reqItem.Item.Name = _storeService.GetItem(fc["ReqItemModel[" + i + "].Item.Code"]).Name;
                        //    reqItem.Quantity = Int32.Parse(fc["ReqItemModel[" + i + "].Quantity"]);
                        //    reqItem.ChargeCode.Code = fc["ReqItemModel[" + i + "].ChargeCode.Code"];

                        //    newModel.Items.Add(reqItem);
                        //}

                        //ViewData["Items"] = itemList;
                        //Items = newModel.Items.ToList();

                        //return View(newModel);
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
                            if (storeItem.Quantity >= reqItem.Quantity)
                            {
                                requisition.Items.Add(reqItem);
                                
                            }
                            else if (storeItem.Quantity == 0)
                            {
                                unavailable++;
                                message += storeItem.Name + " is not available in the store right now. " ;
                                //Log missing item
                            }
                            else if (storeItem.Quantity < reqItem.Quantity)
                            {
                                unavailable++;
                                message += "The quantity of " + storeItem.Name + " that you ordered for is less than what is available in the store right now. ";
                               if(storeItem.Quantity > 1)
                                    message += "There are only "+ storeItem.Quantity + " units left.";
                               else if (storeItem.Quantity == 1)
                                   message += "There is only " + storeItem.Quantity + " unit left.";
                            }
                        }
                    }
                    ////i just added this first if again.It checks if some items are unavailable
                    //if (unavailable > 0 && requisition.Items.Count > 0)
                    //{
                    //    TempData["Message"] = message;
                    //    TempData["Requisition"] = requisition;
                    //    return View(model);
                    //    //return RedirectToAction("Create", model);
                    //}
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
                            catch (Exception e)
                            {
                                Error error = new Error() { Details = e.Message, TimeStamp = DateTime.Now.ToShortTimeString() };
                                _errorService.LogError(error);
                                return RedirectToAction("Index", "Error");
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
                            //var requestor = _staffService.GetUserByUsername(false, requisition.RequestorID);
                            //var mgr = _staffService.GetUserByUsername(false, requisition.ManagerID);
                            //string mgrEmail = mgr != null ? mgr.Email : "";
                            //string reqEmail = requestor != null ? requestor.Email : "";

                            string msg = "A requisition has been made that requires your approval. Click <a href='" + GetUrl() + "Requisition/Response'" + ">Here</a> to view the details and respond.";

                            msg += "<table border=1><th>Item</th><th>Quantity</th><th>Charge Code</th>";

                            foreach (var item in requisition.Items)
                            {
                                string itemName = _storeService.GetItem(item.Item).Name;
                                msg += "<tr><td>" + itemName + "</td><td>" + item.Quantity + "</td><td>" +     item.ChargeCode + "</td></tr>";
                            }
                                
                            msg += "</table>";
                            
                            //_emailService.SendEmail(requisition.ManagerID + "@kpmg.com", requisition.RequestorID + "@kpmg.com", "", "Requisition Approval Request", msg);
                           _emailService.SendEmail(requisition.ManagerID, requisition.RequestorID, requisition.RequestorID + "@kpmg.com;" + requisition.ManagerID + "@kpmg.com", "Requisition Approval Request", msg);

                        }
                        catch { }

                        message += "Your requisition has been created successfully. Your ID is " + requisition.Id;
                        TempData["Message"] = message;
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
            catch (Exception e)
            {
                Error error = new Error() { Details = e.Message, TimeStamp = DateTime.Now.ToShortTimeString() };
                _errorService.LogError(error);
                TempData["Message"] = " An error has occured. Please try again or contact your administrator";
                return RedirectToAction("Create");
            }
        }


        //[Authorize(Roles = "Manager")]
        public ActionResult Respond(int requisitionId)
        {
            //var username = User.Identity.Name;
            //var manager = _staffService.GetUserByUsername(false, requisition.ManagerID);
            //var requestor = _staffService.GetUserByUsername(false, requisition.RequestorID);

            //if (!Roles.IsUserInRole(User.Identity.Name.Remove(0, 3), "Manager"))
            //{
            //    return RedirectToAction("Security", "AccessDenied");
            //}

            var requisition = _reqService.GetRequisition(requisitionId, user.Name.Remove(0,3));
            
            var model = new RequisitionModel()
            {
                Id = requisition.Id,
                Manager = requisition.ManagerID,// manager != null ? manager.DisplayName : "",
                ReqDate = requisition.ReqDate,
                Requestor = requisition.RequestorID,// requestor != null ? requestor.DisplayName : "",
                StatusID = requisition.StatusID,
                StatusDate = requisition.StatusDate,
                UnitID = requisition.UnitID,
                Description = requisition.Description
            };

            foreach(var item in requisition.Items)
            {
                var itmModel = new ReqItemModel();

                itmModel.Id = item.Id;
                itmModel.Quantity = item.Quantity;
                itmModel.Item.Name = _storeService.GetItem(item.Item).Name;
                itmModel.ChargeCode.Code = item.ChargeCode;

                model.Items.Add(itmModel);
                
            }

            return View(model);// Change this to a list of outstanding approvals;
        }

        [HttpPost]
        public ActionResult Respond(FormCollection fc, RequisitionModel model)
        {
            string buttonClicked = Request.Form["SubmitButton"];

            int requisitionId = Int32.Parse(fc["Id"]);
            var requisition = _reqService.GetRequisition(requisitionId, "");

            //if one request is opened in 2 windows, you should not be able to approve or reject on both windows
            if (requisition.StatusID == (int)SystemEnums.Status.ManagerApproved) 
            {
                TempData["Message"] = "This requisition has already been approved. Please contact the store if you require clarifications.";

                return RedirectToAction("Response");
            }
            else if (requisition.StatusID == (int)SystemEnums.Status.ManagerCancelled)
            {
                TempData["Message"] = "This requisition has already been cancelled. Please contact the store if you require clarifications.";

                return RedirectToAction("Response");
            }
            else if (requisition.StatusID == (int)SystemEnums.Status.UserCancelled)
            {
                TempData["Message"] = "This requisition has been cancelled by the requestor. Please contact the store if you require clarifications.";

                return RedirectToAction("Response");
            }
            else if (requisition.StatusID == (int)SystemEnums.Status.Completed)
            {
                TempData["Message"] = "This requisition has been completed. Please contact the store if you require clarifications.";

                return RedirectToAction("Response");
            }
            else if (requisition.StatusID == (int)SystemEnums.Status.StoreCancelled)
            {
                TempData["Message"] = "This requisition has already been declined at the store. Please contact the store if you require clarifications.";

                return RedirectToAction("Response");
            }

            if (buttonClicked == "Approve")
            {
                requisition.StatusID = (int)SystemEnums.Status.ManagerApproved;
                requisition.StatusDate = DateTime.Now;
                requisition.Description = model.Description ?? "";

                TempData["Message"] = "Requisition approved";
            }
            else if(buttonClicked == "Reject")
            {
                requisition.StatusID = (int)SystemEnums.Status.ManagerCancelled;
                requisition.StatusDate = DateTime.Now;
                requisition.Description = model.Description ?? "";

                TempData["Message"] = "Requisition declined";
                //requisition.ManagerName = UserPrincipal.Current.DisplayName;                
                //return item to store       
                foreach (var item in requisition.Items)
                {
                    var itm = _storeService.GetItem(item.Item);
                    itm.Quantity += item.Quantity;
                    _storeService.SaveItem(itm);
                }
            }

            _reqService.SaveRequisition(requisition);
            _auditService.LogRequisitionActivity(requisition, buttonClicked == "Approve" ? SystemEnums.AuditAction.ApprovedRequisition : SystemEnums.AuditAction.RejectedRequisition);

            //TODO: Send Email
            try
            {
                //var requestor = _staffService.GetUserByUsername(false, requisition.RequestorID);
                //var mgr = _staffService.GetUserByUsername(false, requisition.ManagerID);
                //string mgrEmail = mgr != null ? mgr.Email : "";
                //string reqEmail = requestor != null ? requestor.Email : "";

                string msg = "The manager has responded to your request. Click <a href='" + GetUrl() + "Requisition/History'" + ">Here</a> to view the details. \n \n If approved, please proceed to the store to pick up your item(s).";

                if (requisition.StatusID == (int)SystemEnums.Status.ManagerApproved)
                {
                    msg = "The manager has approved your request. Please proceed to the store to pick up your item(s).";
                }
                else if(requisition.StatusID == (int)SystemEnums.Status.ManagerCancelled)
                {
                    msg = "The manager has declined your request. \n Manager's comment: " + requisition.Description;
                }

                //_emailService.SendEmail(requisition.RequestorID + "@kpmg.com", requisition.ManagerID + "@kpmg.com", "", "Requisition Approval Request", msg);
               _emailService.SendEmail(requisition.RequestorID, requisition.ManagerID, requisition.RequestorID + "@kpmg.com;" + requisition.ManagerID + "@kpmg.com", "Requisition Approval Request Response", msg);
            }
            catch { }

            return RedirectToAction("Response");
        }

        public ActionResult Replay(int requisitionId)
        {
            //var username = User.Identity.Name;

            var requisition = _reqService.GetRequisitionByRequestor(requisitionId, user.Name.Remove(0,3));

            //var manager = _staffService.GetUserByUsername(false, requisition.ManagerID);
            //var requestor = _staffService.GetUserByUsername(false, requisition.RequestorID);

            var model = new RequisitionModel()
            {
                Id = requisition.Id,
                Manager = requisition != null ? requisition.ManagerID : "",
                ReqDate = requisition.ReqDate,
                Requestor = requisition != null ? requisition.RequestorID : "",
                StatusID = requisition.StatusID,
                StatusDate = requisition.StatusDate,
                UnitID = requisition.UnitID
            };

            foreach (var item in requisition.Items)
            {
                var itmModel = new ReqItemModel();

                itmModel.Id = item.Id;
                itmModel.Quantity = item.Quantity;
                itmModel.Item.Name = _storeService.GetItem(item.Item).Name;
                itmModel.ChargeCode.Code = item.ChargeCode;

                model.Items.Add(itmModel);

            }
            var items = _storeService.GetItems(false, -1);


            //var staff = _staffService.GetStaffByEmpCode(false, user.EmployeeId);
            //var staff = _staffService.GetUserByUsername(false, user.Name.Remove(0,3));
            var managers = _staffService.GetManagersByDepartment(false, "");

            var chargeableCodes = _setupService.GetChargeableChargeCodes(false);
            var nonChargeableCodes = _setupService.GetNonChargeableChargeCodes(false, "");
            var staffModel = new StaffModel();

            model.Managers.Add (new SelectListItem() { Text = "--Select Manager--", Value = "-1", Selected = true});
            foreach (var m in managers)
            {
                model.Managers.Add(new SelectListItem() { Text = m.DisplayName, Value = m.Username, Selected = (model.Manager ==  requisition.ManagerID)});
            }

            var itemList = new List<ItemModel>();

            foreach (var itm in items)
            {
                itemList.Add(new ItemModel() { Id = itm.Id, Name = itm.Name, Quantity = itm.Quantity, Code = itm.Code });
            }

            var chargeCodeList = new List<ChargeCodeModel>();

            chargeCodeList.Add(new ChargeCodeModel() { Id = -1, Code = user.Name.Remove(0,3) });// EmployeeId });

            foreach (var cc in chargeableCodes)
            {
                chargeCodeList.Add(new ChargeCodeModel() { Id = cc.Id, Code = cc.Code });
            }

            foreach (var cc in nonChargeableCodes)
            {
                chargeCodeList.Add(new ChargeCodeModel() { Id = cc.Id, Code = cc.Code });
            }
            ViewData["Items"] = itemList;
            ViewData["DefaultItem"] = itemList.FirstOrDefault();

            ViewData["ChargeCodes"] = chargeCodeList;
            ViewData["DefaultChargeCode"] = chargeCodeList.FirstOrDefault();

            return View(model);// Change this to a list of outstanding approvals;
        }

        [HttpPost]
        public ActionResult Replay(RequisitionModel model, FormCollection fc)
        {
            try
            {
                //TempData["Message"] = "";
                int count = fc.Count / 3; //3 because we have 3 columns in the table
                string message = null;
                int unavailable = 0;

                if (count >= 1)
                {                    
                    var requisition = _reqService.GetRequisitionByRequestor((int)model.Id, "");
                    requisition.ManagerID = fc["Manager"];
                    requisition.ReqDate = DateTime.Today;
                    requisition.StatusDate = DateTime.Today;
                    requisition.StatusID = (int)SystemEnums.Status.AwaitingMgrApproval;
                    requisition.Items = new List<Req_Item>();

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
                            if (storeItem.Quantity >= reqItem.Quantity)
                            {
                                requisition.Items.Add(reqItem);

                            }
                            else if (storeItem.Quantity == 0)
                            {
                                unavailable++;
                                message += storeItem.Name + " is not available in the store right now. ";
                                //Log missing item
                            }
                            else if (storeItem.Quantity < reqItem.Quantity)
                            {
                                unavailable++;
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
                            var requestor = _staffService.GetUserByUsername(false, requisition.RequestorID);
                            var mgr = _staffService.GetUserByUsername(false, requisition.ManagerID);
                            string mgrEmail = mgr != null ? mgr.Email : "";
                            string reqEmail = requestor != null ? requestor.Email : "";

                            string msg = "A requisition has been made that requires your approval. Click <a href='" + GetUrl() + "Requisition/Response'" + ">Here</a> to view the details and respond";

                            //_emailService.SendEmail(requisition.ManagerID + "@kpmg.com", requisition.RequestorID + "@kpmg.com", "", "Requisition Approval Request", msg);
                            _emailService.SendEmail(requisition.ManagerID, requisition.RequestorID, requisition.RequestorID + "@kpmg.com;" + requisition.ManagerID + "@kpmg.com", "Requisition Approval Request", msg);
                        }
                        catch { }

                        message += "Your requisition has been created Successfully. Your ID is " + requisition.Id;
                        TempData["Message"] = message;
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
            catch (Exception e)
            {
                Error error = new Error() { Details = e.Message, TimeStamp = DateTime.Now.ToShortTimeString() };
                _errorService.LogError(error);
                TempData["Message"] = " An error has occured. Please try again or contact your administrator";
                return RedirectToAction("Create");
            }
        }

        public ActionResult Adjust()
        {
            //var username = User.Identity.Name;

            // var requisition = this.requisition;
            Requisition requisition = TempData["Requisition"] as Requisition;

            var manager = _staffService.GetStaffByUsername(false, requisition.ManagerID);
            var requestor = _staffService.GetStaffByUsername(false, requisition.RequestorID);

            var model = new RequisitionModel()
            {
                //Id = requisition.Id,
                //Manager = requisition.ManagerID,//manager != null ? manager.EmpName : "",
                //ReqDate = requisition.ReqDate,
                //Requestor = requestor != null ? requestor.EmpName : "",
                //StatusID = requisition.StatusID,
                //StatusDate = requisition.StatusDate,
                //UnitID = requisition.UnitID
            };

            foreach (var item in requisition.Items)
            {
                var itmModel = new ReqItemModel();

                itmModel.Id = item.Id;
                itmModel.Quantity = item.Quantity;
                itmModel.ItemName = _storeService.GetItem(item.Item).Name;
                itmModel.ChargeCode.Code = item.ChargeCode;

                model.Items.Add(itmModel);

            }

            return View(model);// Change this to a list of outstanding approvals;
        }

        [HttpPost]
        public ActionResult Adjust(FormCollection fc, RequisitionModel model)
        {
            string buttonClicked = Request.Form["SubmitButton"];

            //int requisitionId = Int32.Parse(fc["Id"]);
            //var requisition = _reqService.GetRequisition(requisitionId, user.Name);

            if (buttonClicked == "Approve")
            {
                var requisition = TempData["Requisition"] as Requisition;
                requisition.StatusID = (int)SystemEnums.Status.AwaitingMgrApproval;
                requisition.StatusDate = DateTime.Now;
                //requisition.ManagerID = requisition.ManagerID;
                //requisition.ManagerName = UserPrincipal.Current.DisplayName;
                _reqService.SaveRequisition(requisition);
                _auditService.LogRequisitionActivity(requisition, SystemEnums.AuditAction.MadeRequisition);
            }
            else if (buttonClicked == "Reject")
            {
                //requisition.StatusID = (int)SystemEnums.Status.ManagerCancelled;
                //requisition.StatusDate = DateTime.Now;
                //requisition.ManagerID = user.Name;
                //requisition.ManagerName = UserPrincipal.Current.DisplayName;   
                return RedirectToAction("Create");
            }

            
            //Log activity
            //Send Email

            return RedirectToAction("Create");
        }

        public ActionResult Responses()
        {
            return View();
        }

        //[Authorize(Roles = "Manager")]
        public ActionResult Response()
        {
            if (!Roles.IsUserInRole(User.Identity.Name.Remove(0, 3), "Manager"))
            {
                return RedirectToAction("Security", "AccessDenied");
            }

            var requisitions = _reqService.GetRequisitions((int)SystemEnums.Status.AwaitingMgrApproval, user.Name.Remove(0, 3));

            var _data = new List<RequisitionModel>();
            foreach (var req in requisitions)
            {
                //var manager = _staffService.GetUserByUsername(false, req.ManagerID);
                //var requestor = _staffService.GetUserByUsername(false, req.RequestorID);
                var reqModel = new RequisitionModel()
                {
                    Id = req.Id,
                    Manager = req.ManagerID,//manager != null ? manager.DisplayName : "",// req.ManagerName,
                    ReqDate = req.ReqDate,
                    Requestor = req.RequestorID,// requestor != null ? requestor.DisplayName : "",
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
            return View(_data);
        }

        public ActionResult History1()
        {
            return View();
        }

        public ActionResult History()
        {
            var requisitions = _reqService.GetRequisitionsByRequestor(-1, user.Name.Remove(0, 3));

            var _data = new List<RequisitionModel>();
            foreach (var req in requisitions)
            {
                var manager = _staffService.GetUserByUsername(false, req.ManagerID);
                var requestor = _staffService.GetUserByUsername(false, req.RequestorID);
                var reqModel = new RequisitionModel()
                {
                    Id = req.Id,
                    Manager = manager != null ? manager.DisplayName : "",// req.ManagerName,
                    ReqDate = req.ReqDate,
                    Requestor = requestor != null ? requestor.DisplayName : "",
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

            return View(_data);
        }

        public ActionResult Req_Read([DataSourceRequest] DataSourceRequest request)
        {
            var requisitions = _reqService.GetRequisitionsByRequestor(-1, user.Name.Remove(0, 3));

            var _data = new List<RequisitionModel>();
            foreach (var req in requisitions)
            {
                var manager = _staffService.GetUserByUsername(false, req.ManagerID);
                var requestor = _staffService.GetUserByUsername(false, req.RequestorID);
                var reqModel = new RequisitionModel()
                {
                    Id = req.Id,
                    Manager = manager != null ? manager.DisplayName : "",// req.ManagerName,
                    ReqDate = req.ReqDate,
                    Requestor = requestor != null ? requestor.DisplayName : "",
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
                        var req = _reqService.GetRequisition(Convert.ToInt32(reqModel.Id), user.Name.Remove(0,3));
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
                var requisition = _reqService.GetRequisitionByRequestor(id, user.Name.Remove(0,3));
                requisition.StatusID = (int)SystemEnums.Status.UserCancelled;
                requisition.StatusDate = DateTime.Now;
                _reqService.SaveRequisition(requisition);

                try
                {
                    _auditService.LogRequisitionActivity(requisition, SystemEnums.AuditAction.CancelledRequisition);
                }
                catch { }

                //return item to store
                foreach(var item in requisition.Items)
                {
                    var itm = _storeService.GetItem(item.Item);
                    itm.Quantity += item.Quantity;
                    _storeService.SaveItem(itm);
                }

                //return Json(id, JsonRequestBehavior.AllowGet);

                return RedirectToAction("History");
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
                var requisition = _reqService.GetRequisitionByRequestor(id, user.Name.Remove(0,3));
                requisition.StatusID = (int)SystemEnums.Status.Completed;
                requisition.StatusDate = DateTime.Now;
                _reqService.SaveRequisition(requisition);

                try
                {
                    _auditService.LogRequisitionActivity(requisition, SystemEnums.AuditAction.Completed);
                }
                catch { }

                return Json(id, JsonRequestBehavior.AllowGet);

                //return RedirectToAction("History");
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
            var requisitions = _reqService.GetRequisitions((int)SystemEnums.Status.AwaitingMgrApproval, user.Name.Remove(0,3));

            var _data = new List<RequisitionModel>();
            foreach (var req in requisitions)
            {
                var manager = _staffService.GetUserByUsername(false, req.ManagerID);
                var requestor = _staffService.GetUserByUsername(false, req.RequestorID);
                var reqModel = new RequisitionModel()
                {
                    Id = req.Id,
                    Manager = manager != null ? manager.DisplayName : "",// req.ManagerName,
                    ReqDate = req.ReqDate,
                    Requestor = requestor != null ? requestor.DisplayName : "",
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

        //[HttpPost]
        public ActionResult ApproveRequisition(int id)
        {
            try
            {
                var requisition = _reqService.GetRequisition(id, user.Name.Remove(0,3));
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

                return Json(id, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Response");
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
                var requisition = _reqService.GetRequisition(id,user.Name.Remove(0,3));
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
                return Json(id, JsonRequestBehavior.AllowGet);

                // return RedirectToAction("Response");
            }

            catch
            {
                return View();
            }
        }

        public ActionResult FilterMenuCustomization_Requestor()
        {
            var db = _reqService.GetRequisitions(-1, user.Name.Remove(0,3));
            //var model = new RequisitionModel();

            //foreach(var min model )
            return Json(db.Select(e => e.RequestorID).Distinct(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterMenuCustomization_Status()
        {
            var db = _reqService.GetRequisitions(-1, user.Name.Remove(0,3));
            var models = new List<RequisitionModel>();

            foreach (var req in db)
            {
                var manager = _staffService.GetUserByUsername(false, req.ManagerID);
                var requestor = _staffService.GetUserByUsername(false, req.RequestorID);
                models.Add(new RequisitionModel()
                {
                    Id = req.Id,
                    Manager = manager != null ? manager.DisplayName : "",
                    ReqDate = req.ReqDate,
                    Requestor = requestor != null ? requestor.DisplayName : "",
                    StatusID = req.StatusID,
                    //Status = ((SystemEnums.Status)(req.StatusID)).ToString(),
                    StatusDate = req.StatusDate
                });
            }
            return Json(models.Select(e => e.StatusString).Distinct(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewItems(int requisitionId)
        {
            //var username = User.Identity.Name;

            var requisition = _reqService.GetRequisitionByRequestor(requisitionId, user.Name.Remove(0, 3));

            //var manager = _staffService.GetUserByUsername(false, requisition.ManagerID);
            //var requestor = _staffService.GetUserByUsername(false, requisition.RequestorID);

            var model = new RequisitionModel()
            {
                Id = requisition.Id,
                Manager = requisition.ManagerID,// manager != null ? manager.DisplayName : "",
                ReqDate = requisition.ReqDate,
                Requestor = requisition.RequestorID,// requestor != null ? requestor.DisplayName : "",
                StatusID = requisition.StatusID,
                StatusDate = requisition.StatusDate,
                UnitID = requisition.UnitID,
                Description = requisition.Description

            };

            foreach (var item in requisition.Items)
            {
                var itmModel = new ReqItemModel();

                itmModel.Id = item.Id;
                itmModel.Quantity = item.Quantity;
                itmModel.Item.Name = _storeService.GetItem(item.Item).Name;
                itmModel.ChargeCode.Code = item.ChargeCode;

                model.Items.Add(itmModel);

            }

            return View(model);// Change this to a list of outstanding approvals;
        }

        [HttpPost]
        public ActionResult ViewItems(FormCollection fc, RequisitionModel model)
        {
            string buttonClicked = Request.Form["SubmitButton"];

            int requisitionId = Int32.Parse(fc["Id"]);
            var requisition = _reqService.GetRequisitionByRequestor(requisitionId, user.Name.Remove(0, 3));

            if (buttonClicked == "Cancel")
            {
                requisition.StatusID = (int)SystemEnums.Status.UserCancelled;
                requisition.StatusDate = DateTime.Now;
                requisition.Description = model.Description ?? "";
            }
            
            _reqService.SaveRequisition(requisition);
            try
            {
                _auditService.LogRequisitionActivity(requisition, SystemEnums.AuditAction.CancelledRequisition);
            }
            catch { }

            foreach (var item in requisition.Items)
            {
                var itm = _storeService.GetItem(item.Item);
                itm.Quantity += item.Quantity;
                _storeService.SaveItem(itm);
            }

            //TODO: Send Email

            return RedirectToAction("History");
        }
        //public ActionResult Store()
        //{
        //    return View();
        //}

        //public ActionResult FilterMenuCustomization_Manager()
        //{
        //    var db = _reqService.GetRequisitionsByRequestor((int)SystemEnums.Status.ManagerApproved, user.Name.Remove(0,3));
        //    var models = new List<RequisitionModel>();

        //    foreach (var req in db)
        //    {
        //        var manager = _staffService.GetUserByUsername(false, req.ManagerID);
        //        var requestor = _staffService.GetUserByUsername(false, req.RequestorID);
        //        models.Add(new RequisitionModel()
        //        {
        //            Id = req.Id,
        //            Manager = manager != null ? manager.DisplayName : "",
        //            ReqDate = req.ReqDate,
        //            Requestor = requestor != null ? requestor.DisplayName : "",
        //            StatusID = req.StatusID,
        //            //Status = ((SystemEnums.Status)(req.StatusID)).ToString(),
        //            StatusDate = req.StatusDate
        //        });
        //    }
        //    return Json(models.Select(e => e.Manager).Distinct(), JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Personal()
        {
            //var check = User.IsInRole("Manager");
            var model = new RequisitionModel();

            var items = _storeService.GetItems(false, -1);

            if (user == null)
            {
                ViewData["Name"] = "null";
                return RedirectToAction("Index", "Home");
            }

            //var staff = _staffService.GetUserByUsername(false, user.Name.Remove(0,3));

            var managers = _staffService.GetManagersByDepartment(false, "");//staff.Department);


           // var chargeableCodes = _setupService.GetChargeableChargeCodes(false);
            //var nonChargeableCodes = _setupService.GetNonChargeableChargeCodes(false, "");// staff.Department);
            //var staffModel = new StaffModel();

            //model.Managers.Add(new SelectListItem() { Text = "--Select Manager--", Value = "-1", Selected = true });
            //foreach (var m in managers)
            //{
            //    if (m.Username != user.Name.Remove(0, 3))
            //        model.Managers.Add(new SelectListItem() { Text = m.DisplayName, Value = m.Username });

            //}
            var itemList = new List<ItemModel>();

            foreach (var itm in items)
            {
                itemList.Add(new ItemModel() { Id = itm.Id, Name = itm.Name, Quantity = itm.Quantity, Code = itm.Code });
            }

            var chargeCodeList = new List<ChargeCodeModel>();

            //make the employee the default first chargecode
            chargeCodeList.Add(new ChargeCodeModel() { Id = -1, Code = user.Name.Remove(0, 3) });//.EmployeeId });
            //chargeCodeList.Add(new ChargeCodeModel() { Id = -1, Code = staff.Username });//.EmployeeId });

            //foreach (var cc in chargeableCodes)
            //{
            //    chargeCodeList.Add(new ChargeCodeModel() { Id = cc.Id, Code = cc.Code });
            //}

            //foreach (var cc in nonChargeableCodes)
            //{
            //    chargeCodeList.Add(new ChargeCodeModel() { Id = cc.Id, Code = cc.Code });
            //}
            ViewData["Items"] = itemList;
            ViewData["DefaultItem"] = itemList.FirstOrDefault();

            ViewData["ChargeCodes"] = chargeCodeList;
            ViewData["DefaultChargeCode"] = chargeCodeList.FirstOrDefault();

            return View(model);
        }

        [HttpPost]
        public ActionResult Personal(RequisitionModel model, FormCollection fc)
        {
            try
            {
                //TempData["Message"] = "";

                int count = fc.Count / 3; //3 because we have 3 columns in the table
                string message = null;
                int unavailable = 0;

                if (count >= 1)
                {
                    var requisition = new Requisition()
                    {
                        ManagerID = user.Name.Remove(0, 3),
                        ReqDate = DateTime.Today,
                        StatusDate = DateTime.Today,
                        UnitID = 1,
                        RequestorID = user.Name.Remove(0, 3),
                        StatusID = (int)SystemEnums.Status.ManagerApproved,
                        IsDeleted = false,
                        SentToAccounts = false,
                        PostedByAccounts = false,
                        Items = new List<Req_Item>()
                    };

                    if (requisition.ManagerID == "-1")
                    {
                        TempData["Message"] = "Select a manager";
                        //var newModel = new RequisitionModel();
                        //for (int i = 0; i < count; i++)
                        //{
                        //    var reqItem = new ReqItemModel();
                        //    reqItem.Item.Name = _storeService.GetItem(fc["ReqItemModel[" + i + "].Item.Code"]).Name;
                        //    reqItem.Quantity = Int32.Parse(fc["ReqItemModel[" + i + "].Quantity"]);
                        //    reqItem.ChargeCode.Code = fc["ReqItemModel[" + i + "].ChargeCode.Code"];

                        //    newModel.Items.Add(reqItem);
                        //}

                        //ViewData["Items"] = itemList;
                        //Items = newModel.Items.ToList();

                        //return View(newModel);
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
                                unavailable++;
                                message += storeItem.Name + " is not available in the store right now. ";
                                //Log missing item
                            }
                            else if (storeItem.Quantity < reqItem.Quantity)
                            {
                                unavailable++;
                                message += "The quantity of " + storeItem.Name + " that you ordered for is less than what is available in the store right now. ";
                                if (storeItem.Quantity > 1)
                                    message += "There are only " + storeItem.Quantity + " units left.";
                                else if (storeItem.Quantity == 1)
                                    message += "There is only " + storeItem.Quantity + " unit left.";
                            }
                        }
                    }
                    ////i just added this first if again.It checks if some items are unavailable
                    //if (unavailable > 0 && requisition.Items.Count > 0)
                    //{
                    //    TempData["Message"] = message;
                    //    TempData["Requisition"] = requisition;
                    //    return View(model);
                    //    //return RedirectToAction("Create", model);
                    //}
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
                            var requestor = _staffService.GetUserByUsername(false, requisition.RequestorID);
                            var mgr = _staffService.GetUserByUsername(false, requisition.ManagerID);
                            string mgrEmail = mgr != null ? mgr.Email : "";
                            string reqEmail = requestor != null ? requestor.Email : "";

                            string msg = "Your requisition has been created successfully. Please proceed to the store to pick up your item(s).";

                            msg += "<table border=1><th>Item</th><th>Quantity</th><th>Charge Code</th>";

                            foreach (var item in requisition.Items)
                            {
                                string itemName = _storeService.GetItem(item.Item).Name;
                                msg += "<tr align='center'><td>" + itemName + "</td><td>" + item.Quantity + "</td><td>" + item.ChargeCode + "</td></tr>";
                            }

                            msg += "</table>";
                            //string msg = "A requisition has been made that requires your approval. Click <a href='" + GetUrl() + "Requisition/Response'" + ">Here</a> to view the details and respond";
                            //
                           // _emailService.SendEmail(requisition.ManagerID + "@kpmg.com", requisition.RequestorID + "@kpmg.com", "", "Requisition Approval Request", msg);
                            _emailService.SendEmail(requisition.ManagerID, requisition.RequestorID, requisition.RequestorID + "@kpmg.com;" + requisition.ManagerID + "@kpmg.com", "Requisition Status", msg);

                        }
                        catch { }

                        message += "Your requisition has been created successfully. Please proceed to the store to pick up your item(s). Your ID is " + requisition.Id;
                        TempData["Message"] = message;
                        return RedirectToAction("History");
                    }
                    else
                    {
                        message += " No requisition has been made. Select at least one available item. ";
                        TempData["Message"] = message;
                        return RedirectToAction("Personal");
                    }

                }
                else
                {
                    message += " An error occured while making your requisition. Add at least one item.";
                    TempData["Message"] = message;
                    return RedirectToAction("Personal");
                }

            }
            catch (Exception e)
            {
                Error error = new Error() { Details = e.Message, TimeStamp = DateTime.Now.ToShortTimeString() };
                _errorService.LogError(error);
                TempData["Message"] = " An error has occured. Please try again or contact your administrator";
                return RedirectToAction("Personal");
            }
        }
    }
}
