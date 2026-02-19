using MediClinic.Models;
using MediClinic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MediClinic.Controllers
{
    [Authorize(Roles = "Supplier")]
    public class SupplierController : Controller
    {
        private readonly MediCureContext _context;

        public SupplierController(MediCureContext context)
        {
            _context = context;
        }

        private int GetSupplierId()
        {
            return int.Parse(User.FindFirst("RoleReferenceId")!.Value);
        }

        #region Dashboard

        public async Task<IActionResult> Index()
        {
            int supplierId = GetSupplierId();

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId);

            var vm = new SupplierDashboardVM
            {
                SupplierName = supplier!.SupplierName,
                TotalAssignedPO = await _context.PurchaseOrderHeaders
                    .CountAsync(p => p.SupplierId == supplierId),
                RecentPO = await _context.PurchaseOrderHeaders
                    .Where(p => p.SupplierId == supplierId)
                    .OrderByDescending(p => p.Podate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(vm);
        }

        #endregion

        #region Profile

        public async Task<IActionResult> Profile()
        {
            int supplierId = GetSupplierId();

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId);

            return View(supplier);
        }

        public async Task<IActionResult> EditProfile()
        {
            int supplierId = GetSupplierId();

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId);

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Supplier supplier)
        {
            int supplierId = GetSupplierId();

            if (supplier.SupplierId != supplierId)
                return Unauthorized();

            if (ModelState.IsValid)
            {
                supplier.SupplierStatus = "Active";
                _context.Update(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Profile));
            }

            return View(supplier);
        }

        #endregion

        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(SupplierResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var username = User.Identity.Name;

            var user = _context.Users
                               .FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            if (user.Password != model.OldPassword)
            {
                ModelState.AddModelError("", "Old password is incorrect.");
                return View(model);
            }

            user.Password = model.Password;

            _context.SaveChanges();

            TempData["ShowSuccess"] = true;
            TempData["SuccessMessage"] = "Supplier password changed successfully!";

            return RedirectToAction("Index", "Supplier");
        }

        #region Purchase Orders

        public async Task<IActionResult> PurchaseOrders()
        {
            int supplierId = GetSupplierId();

            var pos = await _context.PurchaseOrderHeaders
                .Where(p => p.SupplierId == supplierId)
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.Podate)
                .ToListAsync();

            return View(pos);
        }

        public async Task<IActionResult> PODetails(int id)
        {
            int supplierId = GetSupplierId();

            var po = await _context.PurchaseOrderHeaders
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseProductLines)
                    .ThenInclude(l => l.Drug)
                .FirstOrDefaultAsync(p => p.Poid == id && p.SupplierId == supplierId);

            if (po == null)
                return NotFound();

            return View(po);
        }

        #endregion
    }
}
