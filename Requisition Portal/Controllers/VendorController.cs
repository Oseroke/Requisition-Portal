using Requisition_Portal.Models;
using RequisitionPortal.BL.Abstracts;
using RequisitionPortal.BL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Requisition_Portal.Controllers
{
    public class VendorController : BaseController
    {
        public IStoreService _storeService;

        public VendorController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        // GET: Vendor
        public ActionResult Index()
        {
            var vendors = _storeService.GetVendors(false);
            var vendorModels = new List<VendorModel>();
            foreach (var vendor in vendors)
            {
                vendorModels.Add(new VendorModel()
                {
                    Id = vendor.Id,
                    Name = vendor.Name,
                    Address = vendor.Address,
                    ContactPerson = vendor.ContactPerson,
                    Email = vendor.Email,
                    PhoneNumber = vendor.PhoneNumber,
                    VendorUID = vendor.VendorUID
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
                model.Address = vendor.Address;
                model.ContactPerson = vendor.ContactPerson;
                model.Email = vendor.Email;
                model.PhoneNumber = vendor.PhoneNumber;
                model.Name = vendor.Name;
                model.VendorUID = vendor.VendorUID;
            }
            return View(model);
        }

        // GET: Vendor/Create
        public ActionResult Create()
        {
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
                    Address = model.Address,
                    ContactPerson = model.ContactPerson,
                    Email = model.Email,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
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
            var vendor = _storeService.GetVendor(false, id);
            var model = new VendorModel();
            if (vendor != null)
            {
                model.Id = vendor.Id;
                model.Address = vendor.Address;
                model.ContactPerson = vendor.ContactPerson;
                model.Email = vendor.Email;
                model.PhoneNumber = vendor.PhoneNumber;
                model.Name = vendor.Name;
                model.VendorUID = vendor.VendorUID;
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
