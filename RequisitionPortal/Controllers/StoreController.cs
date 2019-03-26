using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using RequisitionPortal.Models;
using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Entities;
using RequisitionPortal.BL.Utils;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Principal;
using System.Web.Security;
using NHibernate.Transform;

namespace RequisitionPortal.Controllers
{
    public class StoreController : BaseController
    {
        public IRequisitionService _reqService;
        public IStoreService _storeService;
        public ISetupService _setupService;
        public IStaffService _staffService;
        public IAuditService _auditService;
        public IEmailService _emailService;
        public List<InventoryItemModel> Items;
        public WindowsIdentity user = WindowsIdentity.GetCurrent();

        public StoreController(IRequisitionService reqService, IStoreService storeService, ISetupService setupService, IStaffService staffService, IAuditService auditService, IEmailService emailService)
        {
            _reqService = reqService;
            _storeService = storeService;
            _setupService = setupService;
            _staffService = staffService;
            _auditService = auditService;
            _emailService = emailService;
            //user = UserPrincipal.Current;
            Items = new List<InventoryItemModel>();
        }

        // GET: Store
        //[Authorize(Roles = "Admin")]
        public ActionResult Index()
        {

            return View();
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult Requisitions()
        {
            if (!Roles.IsUserInRole(User.Identity.Name.Remove(0, 3), "Admin"))
            {
                return RedirectToAction("Security", "AccessDenied");
            }

            var requisitions = _reqService.GetRequisitions((int)SystemEnums.Status.ManagerApproved, "");

            var _data = new List<RequisitionModel>();
            foreach (var req in requisitions)
            {
                var manager = _staffService.GetUserByUsername(false, req.ManagerID);
                var requestor = _staffService.GetUserByUsername(false, req.RequestorID);
                var reqModel = new RequisitionModel()
                {
                    Id = req.Id,
                    Manager = manager != null ? manager.DisplayName : "",
                    ReqDate = req.ReqDate,
                    Requestor = requestor != null ? requestor.DisplayName : "",
                    StatusID = req.StatusID,
                    StatusDate = req.StatusDate,
                    Items = new List<ReqItemModel>()
                };

                foreach (var itm in req.Items)
                {
                    var reqItem = new ReqItemModel();

                    reqItem.Id = itm.Id;
                    reqItem.ChargeCode.Code = itm.ChargeCode;
                    reqItem.Description = itm.Description;
                    reqItem.Quantity = itm.Quantity;
                    reqItem.RequisitionID = itm.RequisitionID;
                    reqItem.ItemName = _storeService.GetItem(itm.Item).Name;

                    reqModel.Items.Add(reqItem);
                }

                _data.Add(reqModel);
            }

            return View(_data);
        }

        public ActionResult IncomingRequisitions()
        {
            if (!Roles.IsUserInRole(User.Identity.Name.Remove(0, 3), "Admin"))
            {
                RedirectToAction("Security", "AccessDenied");
            }

            var requisitions = _reqService.GetRequisitions((int)SystemEnums.Status.AwaitingMgrApproval, "");

            var _data = new List<RequisitionModel>();
            foreach (var req in requisitions)
            {
                var manager = _staffService.GetUserByUsername(false, req.ManagerID);
                var requestor = _staffService.GetUserByUsername(false, req.RequestorID);
                var reqModel = new RequisitionModel()
                {
                    Id = req.Id,
                    Manager = manager != null ? manager.DisplayName : "",
                    ReqDate = req.ReqDate,
                    Requestor = requestor != null ? requestor.DisplayName : "",
                    StatusID = req.StatusID,
                    StatusDate = req.StatusDate,
                    Items = new List<ReqItemModel>()
                };

                foreach (var itm in req.Items)
                {
                    var reqItem = new ReqItemModel();

                    reqItem.Id = itm.Id;
                    reqItem.ChargeCode.Code = itm.ChargeCode;
                    reqItem.Description = itm.Description;
                    reqItem.Quantity = itm.Quantity;
                    reqItem.RequisitionID = itm.RequisitionID;
                    reqItem.ItemName = _storeService.GetItem(itm.Item).Name;

                    reqModel.Items.Add(reqItem);
                }

                _data.Add(reqModel);
            }

            return View(_data);
        }
        public ActionResult Req_Read([DataSourceRequest] DataSourceRequest request)
        {
            var requisitions = _reqService.GetRequisitions((int)SystemEnums.Status.ManagerApproved, "");

            var _data = new List<RequisitionModel>();
            foreach (var req in requisitions)
            {
                var manager = _staffService.GetUserByUsername(false, req.ManagerID);
                var requestor = _staffService.GetUserByUsername(false, req.RequestorID);
                var reqModel = new RequisitionModel()
                {
                    Id = req.Id,
                    Manager = manager != null ? manager.DisplayName : "",
                    ReqDate = req.ReqDate,
                    Requestor = requestor != null ? requestor.DisplayName : "",
                    StatusID = req.StatusID,
                    StatusDate = req.StatusDate,
                    Items = new List<ReqItemModel>()
                };

                foreach (var itm in req.Items)
                {
                    var reqItem = new ReqItemModel();

                    reqItem.Id = itm.Id;
                    reqItem.ChargeCode.Code = itm.ChargeCode;
                    reqItem.Description = itm.Description;
                    reqItem.Quantity = itm.Quantity;
                    reqItem.RequisitionID = itm.RequisitionID;
                    reqItem.ItemName = _storeService.GetItem(itm.Item).Name;

                    reqModel.Items.Add(reqItem);
                }

                _data.Add(reqModel);
            }
            return Json(_data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Respond(int requisitionId)
        {
            if (!Roles.IsUserInRole(User.Identity.Name.Remove(0, 3), "Admin"))
            {
                RedirectToAction("Security", "AccessDenied");
            }
            var requisition = _reqService.GetRequisition(requisitionId, "");

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
        public ActionResult Respond(FormCollection fc, RequisitionModel model)
        {
            string buttonClicked = Request.Form["SubmitButton"];

            int requisitionId = Int32.Parse(fc["Id"]);
            var requisition = _reqService.GetRequisition(requisitionId, "");

            //if one request is opened in 2 windows, you should not be able to approve or reject on both windows
            //if (requisition.StatusID == (int)SystemEnums.Status.ManagerApproved)
            //{
            //    TempData["Message"] = "This requisition has already been approved. Please contact the store if you require clarifications.";

            //    return RedirectToAction("Requisitions");
            //}
            if (requisition.StatusID == (int)SystemEnums.Status.ManagerCancelled)
            {
                TempData["Message"] = "This requisition has already been cancelled. Please contact the store if you require clarifications.";

                return RedirectToAction("Requisitions");
            }
            else if (requisition.StatusID == (int)SystemEnums.Status.UserCancelled)
            {
                TempData["Message"] = "This requisition has been cancelled by the requestor. Please contact the store if you require clarifications.";

                return RedirectToAction("Requisitions");
            }
            else if (requisition.StatusID == (int)SystemEnums.Status.Completed)
            {
                TempData["Message"] = "This requisition has been completed. Please contact the store if you require clarifications.";

                return RedirectToAction("Requisitions");
            }
            else if (requisition.StatusID == (int)SystemEnums.Status.StoreCancelled)
            {
                TempData["Message"] = "This requisition has already been declined at the store. Please contact the store if you require clarifications.";

                return RedirectToAction("Requisitions");
            }

            if (buttonClicked == "Complete")
            {
                requisition.StatusID = (int)SystemEnums.Status.Completed;
                requisition.StatusDate = DateTime.Now;
                requisition.Description = model.Description ?? "";

                TempData["Message"] = "Requisition Completed";

                try
                {
                    string msg = "Store indicates that you have collected your requested item.  Send an email to NG-FMRequisition@ng.kpmg.com if this is incorrect, or if you have any clarifications to make.";

                    //var requestor = _staffService.GetUserByUsername(false, requisition.RequestorID);
                    //var mgr = _staffService.GetUserByUsername(false, requisition.ManagerID);
                    //string mgrEmail = mgr != null ? mgr.Email : "";
                    //string reqEmail = requestor != null ? requestor.Email : "";

                    _emailService.SendEmail(requisition.RequestorID, requisition.ManagerID, requisition.RequestorID + "@kpmg.com;" + requisition.ManagerID + "@kpmg.com", "Requisition Complete", msg);

                    //_emailService.SendEmail(requisition.RequestorID + "@kpmg.com", requisition.ManagerID + "@kpmg.com", "", "Requisition Complete", msg);
                }
                catch { }
            }
            else if (buttonClicked == "Decline")
            {
                requisition.StatusID = (int)SystemEnums.Status.StoreCancelled;
                requisition.StatusDate = DateTime.Now;
                requisition.Description = model.Description ?? "";

                //return item to store         
                foreach (var item in requisition.Items)
                {
                    var itm = _storeService.GetItem(item.Item);
                    itm.Quantity += item.Quantity;
                    _storeService.SaveItem(itm);
                }

                TempData["Message"] = "Requisition Declined";

                //Send an email copying the requesting staff and the manager
                try
                {
                    string msg = "Store has declined your request. Send an email to NG-FMRequisition@ng.kpmg.com if this is incorrect, or if you have any clarifications to make.";

                    //_emailService.SendEmail(requisition.RequestorID + "@kpmg.com", requisition.ManagerID + "@kpmg.com", "", "Requisition Declined at the Store", msg);
                    //var requestor = _staffService.GetUserByUsername(false, requisition.RequestorID);
                    //var mgr = _staffService.GetUserByUsername(false, requisition.ManagerID);
                    //string mgrEmail = mgr != null ? mgr.Email : "";
                    //string reqEmail = requestor != null ? requestor.Email : "";

                    _emailService.SendEmail(requisition.RequestorID, requisition.ManagerID, requisition.RequestorID + "@kpmg.com;" + requisition.ManagerID + "@kpmg.com", "Requisition declined at the store", msg);
                }
                catch { }
            }

            _reqService.SaveRequisition(requisition);
            _auditService.LogRequisitionActivity(requisition, buttonClicked == "Complete" ? SystemEnums.AuditAction.Completed : SystemEnums.AuditAction.StoreCancelledRequisition);

            //TODO: Send Email

            return RedirectToAction("Requisitions");
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
                        var req = _reqService.GetRequisition(Convert.ToInt32(reqModel.Id), "");
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
                var requisition = _reqService.GetRequisition(id, "");
                requisition.StatusID = (int)SystemEnums.Status.Completed;//AwaitingAcknowledgement;
                requisition.StatusDate = DateTime.Now;
                _reqService.SaveRequisition(requisition);

                try
                {
                    _auditService.LogRequisitionActivity(requisition, SystemEnums.AuditAction.Completed);// GivenOutItems);
                }
                catch (Exception ex)
                {

                }

                return Json(id, JsonRequestBehavior.AllowGet);

                //return RedirectToAction("Index");
            }

            catch
            {
                return Json(id, JsonRequestBehavior.AllowGet);
                //return View();
            }
        }

        public ActionResult ReadReqItems(long requisitionID, [DataSourceRequest] DataSourceRequest request)
        {
            var items = _reqService.GetRequisitionItems(requisitionID);

            var _data = new List<ReqItemModel>();

            foreach (var item in items)
            {
                var reqItemModel = new ReqItemModel();

                reqItemModel.Id = item.Id;
                reqItemModel.Item.Name = item.Item;
                reqItemModel.ChargeCode.Code = item.ChargeCode;
                reqItemModel.Quantity = item.Quantity;
                reqItemModel.RequisitionID = item.RequisitionID;
                reqItemModel.ItemName = _storeService.GetItem(item.Item).Name;

                _data.Add(reqItemModel);
            }


            return Json(_data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            if (!Roles.IsUserInRole(User.Identity.Name.Remove(0, 3), "Admin"))
            {
                RedirectToAction("Security", "AccessDenied");
            }
            var model = new InventoryModel();

            model.Items.Add(new SelectListItem() { Text = "-Select Item-", Value = "-1" });
            var items = _storeService.GetItems(false, -1);

            var itemList = new List<ItemModel>();

            foreach (var itm in items)
            {
                itemList.Add(new ItemModel() { Id = itm.Id, Name = itm.Name, Quantity = itm.Quantity, Code = itm.Code });
            }

            var vendors = _storeService.GetVendors(false);
            model.Vendors.Add(new SelectListItem() { Text = "-Select Vendor-", Value = "-1" });

            foreach (var vendor in vendors)
            {
                model.Vendors.Add(new SelectListItem() { Text = vendor.Name, Value = vendor.Id.ToString() });
            }

            ViewData["Items"] = itemList;
            ViewData["DefaultItem"] = itemList.FirstOrDefault();
            ViewData["Date"] = DateTime.Today;
            model.PODate = DateTime.Today;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(InventoryModel model, FormCollection fc)
        {
            try
            {
                if (model.VendorID == -1)
                {
                    TempData["Message"] = "Select a vendor";
                    return RedirectToAction("Add");
                }

                int count = fc.Count / 5;
                string message = null;
                int unavailable = 0;

                if (count < 1)
                {
                    TempData["Message"] = "Select at least one item";
                    return RedirectToAction("Add");
                }


                for (int i = 0; i <= count; i++)
                {
                    var storeItem = new StoreItem()
                    {
                        VendorID = model.VendorID,
                        PODate = model.PODate,
                        InvDate = null,
                        InvoiceNumber = "",
                        IsDeleted = false,
                        Quantity = Int32.Parse(fc["InventoryItemModel[" + i + "].Quantity"]),
                        Description = fc["InventoryItemModel[" + i + "].Description"],
                        PONumber = fc["PONumber"],
                        Code = fc["InventoryItemModel[" + i + "].Item.Code"],
                        UnitPrice = Decimal.Parse(fc["InventoryItemModel[" + i + "].UnitPrice"]),
                        StatusID = 1 //PO Raised, StatusID = 2 means Invoice

                    };

                    // storeItem.Amount = storeItem.Quantity * storeItem.UnitPrice;

                    //try
                    //{
                        _storeService.SaveStoreItem(storeItem);
                    //}
                    //catch
                    //{

                    //}
                    try { } //update the items table
                    catch { }
                }

                foreach (var m in model.InventoryItems)
                {
                    var storeItem = new StoreItem()
                    {
                        ItemID = m.ItemID,
                        Quantity = m.Quantity,
                        UnitPrice = m.UnitPrice,
                        IsDeleted = false,
                        Amount = m.UnitPrice * m.Quantity
                    };

                    try
                    {
                        _storeService.SaveStoreItem(storeItem);
                    }
                    catch
                    {

                    }
                }
                // var item = _storeService.GetItem(storeItem.ItemID);
                try
                {
                    //item.Quantity += storeItem.Quantity;
                    //item.UnitPrice = item.UnitPrice >= storeItem.UnitPrice ? item.UnitPrice : storeItem.UnitPrice;

                    //_storeService.SaveItem(item);
                    TempData["Message"] = "Record updated";
                }
                catch
                {

                }
            }
            catch { }
            return RedirectToAction("Add");
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult Stock()
        {
            if (!Roles.IsUserInRole(User.Identity.Name.Remove(0, 3), "Admin"))
            {
                RedirectToAction("Security", "AccessDenied");
            }
            return View();
        }

        public ActionResult StoreRead([DataSourceRequest] DataSourceRequest request)
        {
            var items = _storeService.GetItems(false, -1);

            var _data = new List<ItemModel>();
            foreach (var item in items)
            {
                var model = new ItemModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Code = item.Code,
                    UnitPrice = item.UnitPrice

                };

                _data.Add(model);
            }
            return Json(_data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CancelRequisition(int id)
        {
            try
            {
                var requisition = _reqService.GetRequisition(id, "");// user.Name);
                requisition.StatusID = (int)SystemEnums.Status.StoreCancelled;
                requisition.StatusDate = DateTime.Now;
                _reqService.SaveRequisition(requisition);

                try
                {
                    _auditService.LogRequisitionActivity(requisition, SystemEnums.AuditAction.StoreCancelledRequisition);
                }
                catch (Exception ex)
                {

                }

                //return item to store
                foreach (var item in requisition.Items)
                {
                    var itm = _storeService.GetItem(item.Item);
                    itm.Quantity += item.Quantity;
                    _storeService.SaveItem(itm);
                }
                return Json(id, JsonRequestBehavior.AllowGet);


                //return RedirectToAction("Index");
            }

            catch
            {
                return Json(id, JsonRequestBehavior.AllowGet);
                //return View();
            }
        }

        public ActionResult FilterMenuCustomization_Requestor()
        {
            var db = _reqService.GetRequisitions(-1, "");
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
            return Json(models.Select(e => e.Requestor).Distinct(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterMenuCustomization_Manager()
        {
            var db = _reqService.GetRequisitions((int)SystemEnums.Status.ManagerApproved, "");
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
            return Json(models.Select(e => e.Manager).Distinct(), JsonRequestBehavior.AllowGet);
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult Report()
        {
            if (!Roles.IsUserInRole(User.Identity.Name.Remove(0, 3), "Admin"))
            {
                RedirectToAction("Security", "AccessDenied");
            }

            var units = _setupService.GetAllUnits(false);
            var unitModels = new List<SelectListItem>();

            unitModels.Add(new SelectListItem() { Text = "-All-", Value = "", Selected = true });
            foreach (var unit in units)
            {
                unitModels.Add(new SelectListItem() { Text = unit.Name, Value = unit.ServLineCode });
            }

            ViewData["Units"] = unitModels;
            return View(new RequisitionModel() { StartDate = DateTime.Today, EndDate = DateTime.Today });
        }

        [HttpPost]
        public ActionResult Report(FormCollection fc, RequisitionModel model)
        {
            string buttonClicked = Request.Form["SubmitButton"];

            string startDateString, endDateString = "";
            DateTime startDate, endDate;

            startDateString = fc["startDate"];
            endDateString = fc["endDate"];
            string unit = fc["ddlUnit"];

            if (string.IsNullOrEmpty(startDateString) || string.IsNullOrEmpty(endDateString))
            {
                TempData["Message"] = "Pick start and end dates";
                return View();
            }
            startDate = DateTime.Parse(startDateString);
            endDate = DateTime.Parse(endDateString);

            startDate = model.StartDate;
            endDate = model.EndDate.AddHours(23.99);

            if (startDate > endDate)
            {
                TempData["Message"] = "Start date cannot be later than end date";
                return View();
            }

            

            if (buttonClicked == "Search")
            {
                var test = _reqService.GetCompletedReqItemsByUnit3(false, "", fc["ddlUnit"] ?? "", startDate, endDate);

                model.ReportModels = new List<ReportModel>();

                foreach (var x in test)
                {
                    model.ReportModels.Add(new ReportModel { Id = x.Id, StatusDate = x.StatusDate, Name = x.Name, Quantity = x.Quantity, UnitPrice = x.UnitPrice, ChargeCode = x.ChargeCode, ServLineCode = x.ServLineCode, Unit = x.Unit, Requestor = x.Requestor, Manager = x.Manager });
                }
                
            }
            else if (buttonClicked == "Print")
            {

                try
                {
                    var test = _reqService.GetCompletedReqItemsByUnit3(false, "", fc["ddlUnit"] ?? "", startDate, endDate);
                    var report = new List<ReportViewModel>();

                    foreach (var x in test)
                    {
                        report.Add(new ReportViewModel { Id = x.Id, StatusDate = x.StatusDate, Name = x.Name, Quantity = x.Quantity, UnitPrice = x.UnitPrice, ChargeCode = x.ChargeCode, ServLineCode = x.ServLineCode, Unit = x.Unit, Requestor = x.Requestor, Manager = x.Manager });
                    }

                    var grid = new GridView();
                    grid.DataSource = report;// _reqService.GetCompletedReqItems(false, "", startDate, endDate);
                    grid.DataBind();

                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=Requisition-" + startDate.ToShortDateString() + "-" + endDate.ToShortDateString() + ".xls");
                    Response.ContentType = "application/ms-excel";

                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);

                    grid.RenderControl(htw);

                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }

                catch { }
            }

            var units = _setupService.GetAllUnits(false);
            var unitModels = new List<SelectListItem>();

            unitModels.Add(new SelectListItem() { Text = "-All-", Value = "", Selected = true });
            foreach (var u in units)
            {
                unitModels.Add(new SelectListItem() { Text = u.Name, Value = u.ServLineCode });
            }

            ViewData["Units"] = unitModels;

            return View(model);
        }

        //public ActionResult ReqItem_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    var items = _reqService.GetCompletedReqItems(false, "");
        //    var _data = new List<ReqItemModel>();

        //    foreach (var item in items)
        //    {
        //        var reqItemModel = new ReqItemModel();
        //        var storeItem = _storeService.GetItem(item.Item);


        //        reqItemModel.Id = item.Id;
        //        reqItemModel.Item.Name = item.Item;
        //        reqItemModel.ChargeCode.Code = item.ChargeCode;
        //        reqItemModel.Quantity = item.Quantity;
        //        reqItemModel.RequisitionID = item.RequisitionID;
        //        reqItemModel.ItemName = storeItem.Name;
        //        //reqItemModel.Amount
        //        reqItemModel.Item = new ItemModel() { Name = storeItem.Name, UnitPrice = storeItem.UnitPrice };

        //        _data.Add(reqItemModel);
        //    }


        //    return Json(_data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}
        public ActionResult ReqItem_Read([DataSourceRequest] DataSourceRequest request)
        {
            //var items = _reqService.GetCompletedReqItems(false, "");
            var _data = new List<ReportModel>();
            var items = _reqService.GetCompletedReqItemsByUnit2(false, "", "");

            foreach (var item in items)
            {
                var reportModel = new ReportModel();
                //var storeItem = _storeService.GetItem(item.Item);


                reportModel.Id = item.Id;
                reportModel.Name = item.Name;
                reportModel.ChargeCode = item.ChargeCode;
                reportModel.Quantity = item.Quantity;
                reportModel.UnitPrice = item.UnitPrice;
                reportModel.Requestor = item.Requestor;
                reportModel.Manager = item.Manager;
                reportModel.Unit = item.Unit;
                reportModel.StatusDate = item.StatusDate;
                //reqItemModel.Amount
                //reqItemModel.Item = new ItemModel() { Name = storeItem.Name, UnitPrice = storeItem.UnitPrice };

                _data.Add(reportModel);
            }


            return Json(_data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Item_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<InventoryItemModel> itemModels)
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

        public ActionResult Update()
        {
            var model = new InventoryModel();
            model.InvDate = DateTime.Today;

            //model.Items.Add(new SelectListItem() { Text = "-Select Item-", Value = "-1" });
            //var items = _storeService.GetItems(false, -1);

            //var itemList = new List<ItemModel>();

            //foreach (var itm in items)
            //{
            //    itemList.Add(new ItemModel() { Id = itm.Id, Name = itm.Name, Quantity = itm.Quantity, Code = itm.Code });
            //}

            //var vendors = _storeService.GetVendors(false);
            //model.Vendors.Add(new SelectListItem() { Text = "-Select Vendor-", Value = "-1" });

            //foreach (var vendor in vendors)
            //{
            //    model.Vendors.Add(new SelectListItem() { Text = vendor.Name, Value = vendor.Id.ToString(), Selected = model.VendorID == vendor.Id });
            //}

            //ViewData["Items"] = itemList;
            //ViewData["DefaultItem"] = itemList.FirstOrDefault();
            //ViewData["Date"] = DateTime.Today;
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(InventoryModel model)
        {

            string buttonClicked = Request.Form["SubmitButton"];
            var poNumber = model.PONumber;
            var items = _storeService.GetStoreItems(false, poNumber);

            if (buttonClicked == "Search")
            {

                foreach (var item in items)
                {
                    //var storeItem = new 
                    model.InventoryItems.Add(new InventoryItemModel()
                    {
                        ItemCode = item.Code,
                        Description = item.Description,
                        ItemName = _storeService.GetItem(item.Code).Name,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                }

                return View(model);
            }
            else
            {
                if (string.IsNullOrEmpty(model.InvoiceNumber))
                {
                    TempData["Message"] = "Enter invoice number";
                    return View(model);
                }

                foreach (var item in items)
                {
                    item.InvoiceNumber = model.InvoiceNumber;
                    item.InvDate = model.InvDate;

                    _storeService.SaveStoreItem(item);

                    //after updating the store records, update your stock table

                    var itm = _storeService.GetItem(item.Code);
                    itm.Quantity += item.Quantity;  //increase stock
                    var newAmount = item.UnitPrice * (decimal)1.25;
                    //if the old price is greater than the new price, stick with the old price, else pick new price
                    itm.UnitPrice = itm.UnitPrice > newAmount ? itm.UnitPrice : newAmount;

                    _storeService.SaveItem(itm);
                }

                TempData["Message"] = "Invoice updated";
                return View(model);
            }
        }

        public ActionResult FilterMenuCustomization_Code()
        {
            var db = _storeService.GetItems(false, -1).OrderBy(e => e.Code);
            var models = new List<ItemModel>();

            foreach (var itm in db)
            {
                //var manager = _staffService.GetUserByUsername(false, itm.ManagerID);
                //var requestor = _staffService.GetUserByUsername(false, itm.RequestorID);
                models.Add(new ItemModel()
                {
                    Id = itm.Id,
                    Code = itm.Code

                });
            }

            return Json(models.Select(e => e.Code).Distinct(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterMenuCustomization_Name()
        {
            var db = _storeService.GetItems(false, -1).OrderBy(e => e.Name);
            var models = new List<ItemModel>();

            foreach (var itm in db)
            {
                //var manager = _staffService.GetUserByUsername(false, itm.ManagerID);
                //var requestor = _staffService.GetUserByUsername(false, itm.RequestorID);
                models.Add(new ItemModel()
                {
                    Id = itm.Id,
                    Name = itm.Name

                });
            }
            return Json(models.Select(e => e.Name).Distinct(), JsonRequestBehavior.AllowGet);
        }
        //private async Task<string> SendEmail(string name, string email, string message)
        //{
        //    var mail = new Outlook.Application();
        //}

    }
}