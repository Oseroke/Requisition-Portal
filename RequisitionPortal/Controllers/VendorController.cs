using RequisitionPortal.Models;
using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RequisitionPortal.Controllers
{
    public class VendorController : BaseController
    {
        public IStoreService _storeService;

        public VendorController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            if (!Roles.IsUserInRole(User.Identity.Name.Remove(0, 3), "Admin"))
            {
                RedirectToAction("Security", "AccessDenied");
            }

            var vendors = _storeService.GetVendors(false);
            var vendorModels = new List<VendorModel>();
            foreach (var vendor in vendors)
            {
                vendorModels.Add(new VendorModel()
                {
                    Id = vendor.Id,
                    Name = vendor.Name,
                    AddressLine1 = vendor.AddressLine1,
                    AddressLine2 = vendor.AddressLine2,
                    Contact = vendor.Contact,
                    Email = vendor.Email,
                    Telephone1 = vendor.Telephone1,
                    Telephone2 = vendor.Telephone2,
                    CitySTZip = vendor.CitySTZip,
                    FaxNo = vendor.FaxNumber,
                    TaxIDNo = vendor.TaxIDNo,
                    VendSince = vendor.VendSince,                
                    VendorUID = vendor.VendorUID,
                    Terms = vendor.Terms
                });

            }
            return View(vendorModels);
        }

        // GET: Vendor/Details/5
        public ActionResult Details(int id)
        {
            var vendor = _storeService.GetVendor(false, id);
            var model = new VendorModel();
            if (vendor != null)
            {
                model.Id = vendor.Id;
                model.AddressLine1 = vendor.AddressLine1;
                model.Contact = vendor.Contact;
                model.Email = vendor.Email;
                model.Telephone1 = vendor.Telephone1;
                model.Telephone2 = vendor.Telephone2;
                model.Name = vendor.Name;
                model.VendorUID = vendor.VendorUID;
                model.AddressLine2 = vendor.AddressLine2;
                model.CitySTZip = vendor.CitySTZip;
                model.FaxNo = vendor.FaxNumber;
                model.TaxIDNo = vendor.TaxIDNo;
                model.VendSince = vendor.VendSince;
                model.Terms = vendor.Terms;
            }
            return View(model);
        }

        // GET: Vendor/Create
        public ActionResult Create()
        {
            if (!Roles.IsUserInRole(User.Identity.Name.Remove(0, 3), "Admin"))
            {
                return RedirectToAction("Security", "AccessDenied");
            }
            var model = new VendorModel();
            return View(model);
        }

        // POST: Vendor/Create
        [HttpPost]
        public ActionResult Create(VendorModel model)
        {
            try
            {
                var vendor = new Vendor()
                {
                    VendorUID = model.VendorUID,
                    AddressLine1 = model.AddressLine1,
                    Contact = model.Contact,
                    Email = model.Email,
                    Name = model.Name,
                    Telephone1 = model.Telephone1,
                    Telephone2 = model.Telephone2,
                    TaxIDNo = model.TaxIDNo,
                    AddressLine2 = model.AddressLine2,
                    CitySTZip = model.CitySTZip,
                    FaxNumber = model.FaxNo,
                    Terms = model.Terms,
                    IsDeleted = false
                };

                _storeService.SaveVendor(vendor);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Vendor/Edit/5
        public ActionResult Edit(int id)
        {
            if (!Roles.IsUserInRole(User.Identity.Name.Remove(0, 3), "Admin"))
            {
                RedirectToAction("Security", "AccessDenied");
            }
            var vendor = _storeService.GetVendor(false, id);
            var model = new VendorModel();
            if (vendor != null)
            {
                model.Id = vendor.Id;
                model.AddressLine1 = vendor.AddressLine1;
                model.Contact = vendor.Contact;
                model.Email = vendor.Email;
                model.Telephone1 = vendor.Telephone1;
                model.Telephone2 = vendor.Telephone2;
                model.Name = vendor.Name;
                model.VendorUID = vendor.VendorUID;
                model.AddressLine2 = vendor.AddressLine2;
                model.CitySTZip = vendor.CitySTZip;
                model.FaxNo = vendor.FaxNumber;
                model.TaxIDNo = vendor.TaxIDNo;
                model.VendSince = vendor.VendSince;
                model.Terms = vendor.Terms;
            }
            return View(model);
        }

        // POST: Vendor/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Vendor/Delete/5
        public ActionResult Delete(int id)
        {
            if (!Roles.IsUserInRole(User.Identity.Name.Remove(0, 3), "Admin"))
            {
                RedirectToAction("Security", "AccessDenied");
            }
            return View();
        }

        // POST: Vendor/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
