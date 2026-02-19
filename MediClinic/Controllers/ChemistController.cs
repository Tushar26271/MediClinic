using MediClinic.Models;
using MediClinic.Models.ChemistViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MediClinic.Controllers
{
    [Authorize(Roles = "Chemist")]
    public class ChemistController : Controller
    {
        private readonly MediCureContext _context;

        public ChemistController(MediCureContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int chemistId = int.Parse(User.FindFirst("RoleReferenceId")!.Value);

            var chemist = _context.Chemists
    .FirstOrDefault(p => p.ChemistId == chemistId);
            var vm = new DashboardVM
            {
                TotalDrugs = _context.Drugs.Count(),
                ActiveDrugs = _context.Drugs.Count(d => d.DrugStatus == "Active"),
                PendingRequests = _context.DrugRequests.Count(r => r.RequestStatus == "Pending"),
                TotalPO = _context.PurchaseOrderHeaders.Count()
            };

            return View(vm);

           // return View(chemist);
        }


        #region Dashboard



        #endregion

        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]


        public IActionResult ResetPassword(ChemistResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get logged-in username
            var username = User.Identity.Name;

            var user = _context.Users
                               .FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            // Check old password
            if (user.Password != model.OldPassword)
            {
                ModelState.AddModelError("", "Old password is incorrect.");
                return View(model);
            }

            // Update password
            user.Password = model.Password;

            _context.SaveChanges();

            TempData["ShowSuccess"] = true;
            TempData["SuccessMessage"] = "Password changed successfully!";

            return RedirectToAction("Index", "Chemist");
        }
        private int GetChemistId()
        {
            return int.Parse(User.FindFirst("RoleReferenceId")!.Value);
        }

        public async Task<IActionResult> Profile()
        {
            //var chemistId = HttpContext.Session.GetInt32("ChemistId");

            //if (chemistId == null)
            //    return RedirectToAction("Login", "Account");

            //var chemist = await _context.Chemists
            //    .FirstOrDefaultAsync(c => c.ChemistId == chemistId);

            //if (chemist == null)
            //    return NotFound();

            //return View(chemist);

            int chemistId = GetChemistId();

            var chemist = await _context.Chemists
                .FirstOrDefaultAsync(s => s.ChemistId == chemistId);

            return View(chemist);
        }


        public async Task<IActionResult> EditProfile()
        {
            int chemistId = GetChemistId();

            var chemist = await _context.Chemists
                .FirstOrDefaultAsync(c => c.ChemistId == chemistId);

            return View(chemist);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Chemist chemist)
        {
            int chemistId = GetChemistId();

            if (chemist.ChemistId != chemistId)
                return Unauthorized();

            if (ModelState.IsValid)
            {
                chemist.ChemistStatus = "Active";   // Same like supplier
                _context.Update(chemist);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Profile));
            }

            return View(chemist);
        }


        #region DRUG CRUD

        public async Task<IActionResult> Drugs()
        {
            return View("Drugs/Index", await _context.Drugs.ToListAsync());
        }

        public IActionResult CreateDrug()
        {
            return View("Drugs/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDrug(Drug drug)
        {
            if (ModelState.IsValid)
            {
                drug.DrugStatus = "Active";
                _context.Add(drug);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Drugs));
            }
            return View("Drugs/Create", drug);
        }

        public async Task<IActionResult> EditDrug(int id)
        {
            var drug = await _context.Drugs.FindAsync(id);
            return View("Drugs/Edit", drug);
        }

        [HttpPost]
        public async Task<IActionResult> EditDrug(int id, Drug drug)
        {
            if (id != drug.DrugId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(drug);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Drugs));
            }
            return View("Drugs/Edit", drug);
        }

        public async Task<IActionResult> DeleteDrug(int id)
        {
            var drug = await _context.Drugs.FindAsync(id);
            return View("Drugs/Delete", drug);
        }

        [HttpPost, ActionName("DeleteDrug")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drug = await _context.Drugs.FindAsync(id);
            drug.DrugStatus = "Inactive";
            _context.Drugs.Update(drug);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Drugs));
        }

        #endregion

        #region DRUG REQUEST MANAGEMENT

        public async Task<IActionResult> DrugRequests(string status)
        {
            var query = _context.DrugRequests.Include(r => r.Physician).AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.RequestStatus == status);

            return View("DrugRequests/Index", await query.ToListAsync());
        }

        public async Task<IActionResult> ProcessRequest(int id)
        {
            var request = await _context.DrugRequests.FindAsync(id);
            request.RequestStatus = "Processed";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DrugRequests));
        }

        #endregion

        #region PURCHASE ORDER

        public async Task<IActionResult> PurchaseOrders()
        {
            var data = await _context.PurchaseOrderHeaders
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.Podate)
                .ToListAsync();

            return View("PurchaseOrders/Index", data);
        }

        ///// GET
        //public IActionResult CreatePO()
        //{
        //    var lastPo = _context.PurchaseOrderHeaders
        //        .OrderByDescending(p => p.Poid)
        //        .FirstOrDefault();

        //    string newPoNo = "PO-01";
        //    if (lastPo != null)
        //    {
        //        var lastNo = lastPo.Pono?.Split('-').LastOrDefault();
        //        if (int.TryParse(lastNo, out int n))
        //            newPoNo = $"PO-{(n + 1):D2}";
        //    }

        //    var vm = new PurchaseOrderVM
        //    {
        //        PONo = newPoNo,
        //        PODate = DateTime.Now,
        //        Suppliers = new SelectList(
        //            _context.Suppliers.Where(s => s.SupplierStatus == "Active"),
        //            "SupplierId",
        //            "SupplierName"
        //        ),
        //        ProductLines = new List<PurchaseLineVM> { new PurchaseLineVM { SlNo = 1 } }
        //    };

        //    ViewBag.Drugs = _context.Drugs
        //        .Where(d => d.DrugStatus == "Active")
        //        .Select(d => new { d.DrugId, d.DrugTitle })
        //        .ToList();

        //    return View("PurchaseOrders/Create", vm);

        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreatePO(PurchaseOrderVM vm)
        //{
        //    if (!ModelState.IsValid || vm.ProductLines == null || !vm.ProductLines.Any())
        //    {
        //        vm.Suppliers = new SelectList(
        //            _context.Suppliers.Where(s => s.SupplierStatus == "Active"),
        //            "SupplierId",
        //            "SupplierName"
        //        );
        //        ViewBag.Drugs = _context.Drugs
        //            .Where(d => d.DrugStatus == "Active")
        //            .Select(d => new { d.DrugId, d.DrugTitle })
        //            .ToList();

        //        return View("PurchaseOrders/Create", vm);
        //    }

        //    if (vm.PODate == DateTime.MinValue)
        //        vm.PODate = DateTime.Now;

        //    var header = new PurchaseOrderHeader
        //    {
        //        Pono = vm.PONo,
        //        Podate = vm.PODate,
        //        SupplierId = vm.SupplierId
        //    };

        //    _context.PurchaseOrderHeaders.Add(header);
        //    await _context.SaveChangesAsync();

        //    foreach (var item in vm.ProductLines)
        //    {
        //        _context.PurchaseProductLines.Add(new PurchaseProductLine
        //        {
        //            Poid = header.Poid,
        //            DrugId = item.DrugId,
        //            Qty = item.Qty,
        //            Note = item.Note,
        //            SlNo = item.SlNo
        //        });
        //    }

        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(PurchaseOrders));
        //}
        // GET: CreatePO
        //public IActionResult CreatePO()
        //{
        //    // Auto-generate PONo
        //    var lastPo = _context.PurchaseOrderHeaders
        //        .OrderByDescending(p => p.Poid)
        //        .FirstOrDefault();

        //    string newPoNo = "PO-01";
        //    if (lastPo != null)
        //    {
        //        var lastNo = lastPo.Pono?.Split('-').LastOrDefault();
        //        if (int.TryParse(lastNo, out int n))
        //        {
        //            newPoNo = $"PO-{(n + 1):D2}";
        //        }
        //    }

        //    var vm = new PurchaseOrderVM
        //    {
        //        PONo = newPoNo,
        //        PODate = DateTime.Now,
        //        SupplierId = 0,
        //        Suppliers = new SelectList(
        //            _context.Suppliers.Where(s => s.SupplierStatus == "Active"),
        //            "SupplierId", "SupplierName"
        //        ),
        //        ProductLines = new List<PurchaseLineVM> { new PurchaseLineVM { SlNo = 1 } }
        //    };

        //    ViewBag.Drugs = _context.Drugs
        //        .Where(d => d.DrugStatus == "Active")
        //        .Select(d => new { d.DrugId, d.DrugTitle })
        //        .ToList();

        //    return View("PurchaseOrders/Create", vm);
        //}

        //// POST: CreatePO
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreatePO(PurchaseOrderVM vm)
        //{
        //    if (!ModelState.IsValid || vm.ProductLines == null || !vm.ProductLines.Any())
        //    {
        //        vm.Suppliers = new SelectList(
        //            _context.Suppliers.Where(s => s.SupplierStatus == "Active"),
        //            "SupplierId", "SupplierName"
        //        );

        //        ViewBag.Drugs = _context.Drugs
        //            .Where(d => d.DrugStatus == "Active")
        //            .Select(d => new { d.DrugId, d.DrugTitle })
        //            .ToList();

        //        return View("PurchaseOrders/Create", vm);
        //    }

        //    // Ensure PODate is valid
        //    var poDate = vm.PODate == default ? DateTime.Now : vm.PODate;

        //    var header = new PurchaseOrderHeader
        //    {
        //        Pono = vm.PONo,
        //        Podate = poDate,
        //        SupplierId = vm.SupplierId
        //    };

        //    _context.PurchaseOrderHeaders.Add(header);
        //    await _context.SaveChangesAsync();

        //    foreach (var item in vm.ProductLines)
        //    {
        //        if (item.DrugId > 0 && item.Qty > 0)
        //        {
        //            _context.PurchaseProductLines.Add(new PurchaseProductLine
        //            {
        //                Poid = header.Poid,
        //                DrugId = item.DrugId,
        //                Qty = item.Qty,
        //                Note = item.Note,
        //                SlNo = item.SlNo
        //            });
        //        }
        //    }

        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(PurchaseOrders));
        //}

        //[HttpPost]
        //public async Task<IActionResult> CreateDrugAjax([FromBody] Drug model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        model.DrugStatus = "Active";
        //        _context.Drugs.Add(model);
        //        await _context.SaveChangesAsync();
        //        return Json(new { success = true, drugId = model.DrugId, drugTitle = model.DrugTitle });
        //    }
        //    return Json(new { success = false, message = "Invalid data" });
        //}

        #endregion
        public async Task<IActionResult> PODetails(int id)
        {
            // Fetch the Purchase Order Header along with its PurchaseProductLines, Supplier, and Drugs.
            var po = await _context.PurchaseOrderHeaders
                .Include(p => p.Supplier)  // Include the Supplier data
                .Include(p => p.PurchaseProductLines)  // Include related Product Lines
                    .ThenInclude(pl => pl.Drug)  // Include related Drugs in each Product Line
                .FirstOrDefaultAsync(p => p.Poid == id);  // Find by POID (assuming Poid is the primary key)

            // Check if the purchase order exists.
            if (po == null)
            {
                return NotFound();  // Return 404 if PO not found
            }

            // Return the view with the PurchaseOrderHeader model.
            return View("PurchaseOrders/Details", po);
        }

        // GET: CreatePO
        public IActionResult CreatePO()
        {
            // Auto-generate PONo
            var lastPo = _context.PurchaseOrderHeaders
                .OrderByDescending(p => p.Poid)
                .FirstOrDefault();

            string newPoNo = "PO-01";
            if (lastPo != null)
            {
                var lastNo = lastPo.Pono?.Split('-').LastOrDefault();
                if (int.TryParse(lastNo, out int n))
                {
                    newPoNo = $"PO-{(n + 1):D2}";
                }
            }

            var vm = new PurchaseOrderVM
            {
                PONo = newPoNo,
                PODate = DateTime.Now,
                SupplierId = 0,
                Suppliers = new SelectList(_context.Suppliers.Where(s => s.SupplierStatus == "Active"), "SupplierId", "SupplierName"),
                ProductLines = new List<PurchaseLineVM> { new PurchaseLineVM { SlNo = 1 } }
            };

            ViewBag.Drugs = _context.Drugs
                .Where(d => d.DrugStatus == "Active")
                .Select(d => new { d.DrugId, d.DrugTitle })
                .ToList();

            return View("PurchaseOrders/Create", vm);
        }

        //// POST: CreatePO
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreatePO(PurchaseOrderVM vm)
        //{
        //    // Ensure at least one product line
        //    if (!ModelState.IsValid || vm.ProductLines == null || !vm.ProductLines.Any())
        //    {
        //        vm.Suppliers = new SelectList(_context.Suppliers.Where(s => s.SupplierStatus == "Active"), "SupplierId", "SupplierName");

        //        ViewBag.Drugs = _context.Drugs
        //            .Where(d => d.DrugStatus == "Active")
        //            .Select(d => new { d.DrugId, d.DrugTitle })
        //            .ToList();

        //        // If ProductLines is null (for failed model binding), initialize one
        //        if (vm.ProductLines == null || !vm.ProductLines.Any())
        //        {
        //            vm.ProductLines = new List<PurchaseLineVM> { new PurchaseLineVM { SlNo = 1 } };
        //        }

        //        return View("PurchaseOrders/Create", vm);
        //    }

        //    // Ensure PODate is valid
        //    var poDate = vm.PODate == default ? DateTime.Now : vm.PODate;

        //    var header = new PurchaseOrderHeader
        //    {
        //        Pono = vm.PONo,
        //        Podate = poDate,
        //        SupplierId = vm.SupplierId
        //    };

        //    _context.PurchaseOrderHeaders.Add(header);
        //    await _context.SaveChangesAsync();

        //    int slNo = 1;
        //    foreach (var item in vm.ProductLines)
        //    {
        //        if (item.DrugId > 0 && item.Qty > 0)
        //        {
        //            _context.PurchaseProductLines.Add(new PurchaseProductLine
        //            {
        //                Poid = header.Poid,
        //                DrugId = item.DrugId,
        //                Qty = item.Qty,
        //                Note = item.Note,
        //                SlNo = slNo++
        //            });
        //        }
        //    }

        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(PurchaseOrders));
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreatePO(PurchaseOrderVM vm)
        //{
        //    // Check if ProductLines is null or empty (in case of failed model binding)
        //    if (vm.ProductLines == null || !vm.ProductLines.Any())
        //    {
        //        // Initialize ProductLines if it's null or empty
        //        vm.ProductLines = new List<PurchaseLineVM> { new PurchaseLineVM { SlNo = 1 } };
        //    }

        //    // Ensure there's at least one product line with valid data
        //    if (vm.ProductLines.Any(p => p.DrugId == 0 || p.Qty <= 0)) // assuming DrugId 0 means 'not selected' or invalid quantity
        //    {
        //        // You may want to add a validation error here to stop further processing
        //        ModelState.AddModelError("", "Please ensure all product lines have valid drug and quantity.");
        //        // Re-populate the select list for the view
        //        vm.Suppliers = new SelectList(_context.Suppliers.Where(s => s.SupplierStatus == "Active"), "SupplierId", "SupplierName");
        //        return View("PurchaseOrders/Create", vm);
        //    }

        //    // Create PurchaseOrderHeader (POH)
        //    var header = new PurchaseOrderHeader
        //    {
        //        Pono = vm.PONo,
        //        Podate = vm.PODate,
        //        SupplierId = vm.SupplierId
        //    };

        //    _context.PurchaseOrderHeaders.Add(header);
        //    await _context.SaveChangesAsync();  // Save header first to get the POID

        //    // Add each product line to the PurchaseProductLine table
        //    foreach (var item in vm.ProductLines)
        //    {
        //        if (item.DrugId > 0 && item.Qty > 0)  // Ensure drug and qty are valid
        //        {
        //            _context.PurchaseProductLines.Add(new PurchaseProductLine
        //            {
        //                Poid = header.Poid,  // The POID from the header
        //                DrugId = item.DrugId,
        //                Qty = item.Qty,
        //                Note = item.Note,
        //                SlNo = item.SlNo
        //            });
        //        }
        //    }

        //    // Save the product lines
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(PurchaseOrders));  // Redirect to list after saving
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreatePO(PurchaseOrderVM vm)
        //{
        //    // Ensure PODate is set to a valid value (current date if null)
        //    if (vm.PODate == default(DateTime))  // If it's the default value (01/01/0001 00:00)
        //    {
        //        vm.PODate = DateTime.Now;  // Set it to the current date/time
        //    }

        //    // Ensure ProductLines is initialized if it's empty
        //    if (vm.ProductLines == null || !vm.ProductLines.Any())
        //    {
        //        vm.ProductLines = new List<PurchaseLineVM> { new PurchaseLineVM { SlNo = 1 } };
        //    }

        //    // Validate ProductLines
        //    if (vm.ProductLines.Any(p => p.DrugId == 0 || p.Qty <= 0))  // Assuming 0 means unselected or invalid
        //    {
        //        ModelState.AddModelError("", "Please ensure all product lines have valid drug and quantity.");
        //        vm.Suppliers = new SelectList(_context.Suppliers.Where(s => s.SupplierStatus == "Active"), "SupplierId", "SupplierName");
        //        return View("PurchaseOrders/Create", vm);
        //    }

        //    // Create PurchaseOrderHeader
        //    var header = new PurchaseOrderHeader
        //    {
        //        Pono = vm.PONo,
        //        Podate = vm.PODate,
        //        SupplierId = vm.SupplierId
        //    };

        //    _context.PurchaseOrderHeaders.Add(header);
        //    await _context.SaveChangesAsync();  // Save header first to get POID

        //    // Add each product line to the PurchaseProductLine table
        //    foreach (var item in vm.ProductLines)
        //    {
        //        if (item.DrugId > 0 && item.Qty > 0)  // Ensure valid drug and quantity
        //        {
        //            _context.PurchaseProductLines.Add(new PurchaseProductLine
        //            {
        //                Poid = header.Poid,  // The POID from the header
        //                DrugId = item.DrugId,
        //                Qty = item.Qty,
        //                Note = item.Note,
        //                SlNo = item.SlNo
        //            });
        //        }
        //    }

        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(PurchaseOrders));  // Redirect to list after saving
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePO(PurchaseOrderVM vm)
        {
            // Ensure PODate is set to a valid value (current date if null)
            if (vm.PODate == default(DateTime))  // If it's the default value (01/01/0001 00:00)
            {
                vm.PODate = DateTime.Now;  // Set it to the current date/time
            }

            // Ensure ProductLines is initialized if it's empty
            if (vm.ProductLines == null || !vm.ProductLines.Any())
            {
                vm.ProductLines = new List<PurchaseLineVM> { new PurchaseLineVM { SlNo = 1 } };
            }

            // Set SlNo for each product line
            int slNo = 1;
            foreach (var item in vm.ProductLines)
            {
                if (item.DrugId > 0 && item.Qty > 0)  // Ensure valid drug and quantity
                {
                    item.SlNo = slNo++;  // Increment SlNo for each product line
                }
            }

            // Create PurchaseOrderHeader
            var header = new PurchaseOrderHeader
            {
                Pono = vm.PONo,
                Podate = vm.PODate,
                SupplierId = vm.SupplierId
            };

            _context.PurchaseOrderHeaders.Add(header);
            await _context.SaveChangesAsync();  // Save header first to get POID

            // Add each product line to the PurchaseProductLine table
            foreach (var item in vm.ProductLines)
            {
                if (item.DrugId > 0 && item.Qty > 0)  // Ensure valid drug and quantity
                {
                    _context.PurchaseProductLines.Add(new PurchaseProductLine
                    {
                        Poid = header.Poid,  // The POID from the header
                        DrugId = item.DrugId,
                        Qty = item.Qty,
                        Note = item.Note,
                        SlNo = item.SlNo
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PurchaseOrders));  // Redirect to list after saving
        }

    }
}
