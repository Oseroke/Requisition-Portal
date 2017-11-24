using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Requisition_Portal.Models;
using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Entities;
using RequisitionPortal.BL.Utils;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Requisition_Portal.Controllers
{
    public class StoreController : BaseController
    {
        public IRequisitionService _reqService;
        public IStoreService _storeService;
        public ISetupService _setupService;
        public IStaffService _staffService;
        public IAuditService _auditService;
        public UserPrincipal user = UserPrincipal.Current;

        public StoreController(IRequisitionService reqService, IStoreService storeService, ISetupService setupService, IStaffService staffService, IAuditService auditService)
        {
            _reqService = reqService;
            _storeService = storeService;
            _setupService = setupService;
            _staffService = staffService;
            _auditService = auditService;
        }

        // GET: Store
        public ActionResult Index()
        {
            //var model = new RequisitionModel();
            return View();
        }

        public ActionResult Req_Read([DataSourceRequest] DataSourceRequest request)
        {
            var requisitions = _reqService.GetRequisitions((int)SystemEnums.Status.ManagerApproved, "");

            var _data = new List<RequisitionModel>();
            foreach (var req in requisitions)
            {
                var manager = _staffService.GetStaffByUsername(false, req.ManagerID);
                var requestor = _staffService.GetStaffByUsername(false, req.RequestorID);
                var reqModel = new RequisitionModel()
                {
                    Id = req.Id,
                    Manager = manager != null ? manager.EmpName : "",
                    ReqDate = req.ReqDate,
                    Requestor = requestor != null ? requestor.EmpName : "",
                    StatusID = req.StatusID,
                    //Status = ((SystemEnums.Status)(req.StatusID)).ToString(),
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
                requisition.StatusID = (int)SystemEnums.Status.AwaitingAcknowledgement;
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

        public ActionResult Add()
        {
            var model = new StoreItemModel();
            model.Items.Add(new SelectListItem() { Text = "-Select Item-", Value = "-1" });
            var items = _storeService.GetItems(false, -1);

            foreach (var item in items)
            {
                model.Items.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }

            var vendors = _storeService.GetVendors(false);
            model.Vendors.Add(new SelectListItem() { Text = "-Select Item-", Value = "-1" });

            foreach (var vendor in vendors)
            {
                model.Vendors.Add(new SelectListItem() { Text = vendor.Name, Value = vendor.Id.ToString() });
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Add(StoreItemModel model)
        {
            // Add Logic
            if (model.ItemID == -1)
            {
                TempData["Message"] = "Select one item";
                return RedirectToAction("Add");
            }

            var storeItem = new StoreItem()
            {
                InvoiceNumber = model.InvoiceNumber,
                ItemID = model.ItemID,
                PONumber = model.PONumber,
                Date = DateTime.Today,
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice,
                VendorID = model.VendorID,
                IsDeleted = false,
                Amount = model.UnitPrice * model.Quantity
            };

            try
            {
                _storeService.SaveStoreItem(storeItem);
            }
            catch
            {

            }

            var item = _storeService.GetItem(storeItem.ItemID);
            try
            {
                item.Quantity += storeItem.Quantity;
                item.UnitPrice = storeItem.UnitPrice;

                _storeService.SaveItem(item);
                TempData["Message"] = "Record updated";
            }
            catch
            {

            }
            
            return RedirectToAction("Add");
        }

        public ActionResult Stock()
        {
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
                    Code = item.Code
                    
                };                           

                _data.Add(model);
            }
            return Json(_data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CancelRequisition(int id)
        {
            try
            {
                var requisition = _reqService.GetRequisition(id, user.Name);
                requisition.StatusID = (int)SystemEnums.Status.UserCancelled;
                requisition.StatusDate = DateTime.Now;
                _reqService.SaveRequisition(requisition);

                _auditService.LogRequisitionActivity(requisition, SystemEnums.AuditAction.StoreCancelledRequisition);

                //return item to store
                foreach (var item in requisition.Items)
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

        //private async Task<string> SendEmail(string name, string email, string message)
        //{
        //    var mail = new Outlook.Application();
        //}

        //private async Task<string> SendEmail(string name, string email, string messages, string phone)
        //{
        //    var message = new MailMessage();
        //    message.To.Add(new MailAddress("abc@xyz.com"));  // replace with receiver's email id  
        //    message.From = new MailAddress("xyz@abc.com");  // replace with sender's email id 
        //    message.Subject = "Message From" + email;
        //    message.Body = "Name: " + name + "\nFrom: " + email + "\nPhone: " + phone + "\n" + messages;
        //    message.IsBodyHtml = true;
        //    using (var smtp = new SmtpClient())
        //    {
        //        var credential = new NetworkCredential
        //        {
        //            UserName = "oseroke.igwubor@ng.kpmg.com",  // replace with sender's email id 
        //            Password = ""  // replace with password 
        //        };
        //        smtp.Credentials = credential;
        //        smtp.Host = "smtp-mail.outlook.com";
        //        smtp.Port = 587;
        //        smtp.EnableSsl = true;
        //        await smtp.SendMailAsync(message);
        //        return "sent";
        //    }
        //}
    }
}