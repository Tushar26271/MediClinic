using MediClinic.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MediClinic.Controllers
{
    [Authorize(Roles = "Physician")]
    public class PhysicianController : Controller
    {
        readonly MediCureContext _context;

        public PhysicianController(MediCureContext context)
        {
            _context = context;
        }

        // ================= INDEX =================
        public IActionResult Index()
        {
            int physicianId = int.Parse(User.FindFirst("RoleReferenceId")!.Value);

            var physician = _context.Physicians
                .FirstOrDefault(p => p.PhysicianId == physicianId);

            if (physician == null)
                return NotFound();

            var schedules = _context.Schedules
                // 🔥 REQUIRED FOR CRITICALITY + REASON
                .Include(s => s.Appointment)
                    .ThenInclude(a => a.Patient)
                .Where(s =>
                    s.PhysicianId == physicianId &&
                    s.ScheduleDate >= DateOnly.FromDateTime(DateTime.Today))
                .OrderBy(s => s.ScheduleDate)
                .ToList();

            ViewBag.DoctorName = physician.PhysicianName;
            ViewBag.TotalToday = schedules.Count(s =>
                s.ScheduleDate >= DateOnly.FromDateTime(DateTime.Today));

            return View(schedules);
        }

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
            TempData["SuccessMessage"] = "Physician password changed successfully!";

            return RedirectToAction("Index", "Physician");
        }

        // ================= VIEW SCHEDULE =================
        public IActionResult ViewSchedule()
        {
            var user = User.Identity;

            if (user == null)
                return RedirectToAction("Login", "Account");

            int physicianId = int.Parse(User.FindFirst("RoleReferenceId")!.Value);

            var schedule = _context.Schedules
                .Include(ph => ph.Physician)

                // 🔥 VERY IMPORTANT (loads Appointment fields)
                .Include(a => a.Appointment)
                    .ThenInclude(p => p.Patient)

                .Where(s => s.PhysicianId == physicianId)
                .OrderBy(s => s.ScheduleDate) // ✅ added safe ordering
                .ToList();

            return View(schedule);
        }

        // ================= PATIENT RECORDS =================
        public IActionResult PatientRecords(int id)
        {
            var records = _context.PhysicianAdvices
                .Include(a => a.Schedule)
                    .ThenInclude(s => s.Physician)
                .Include(a => a.Schedule)
                    .ThenInclude(s => s.Appointment)
                        .ThenInclude(p => p.Patient)
                .Include(p => p.PhysicianPrescrips)
                    .ThenInclude(d => d.Drug)
                .Where(s => s.Schedule.Appointment.Patient.PatientId == id)
                .ToList();

            return View(records);
        }

        // ================= DRUG REQUEST =================
        [HttpGet]
        public IActionResult DrugRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DrugRequest(DrugRequest d)
        {
            
            var user = User.Identity;

            DrugRequest obj = new DrugRequest
            {
                RequestDate = DateTime.Today,
                DrugsInfoText = d.DrugsInfoText,
                PhysicianId = int.Parse(User.FindFirst("RoleReferenceId")!.Value),
                RequestStatus = "PENDING"
            };

            _context.DrugRequests.Add(obj);
            _context.SaveChanges();

            ViewBag.SuccessMessage = "Drug Request Submitted Successfully";

            return View(new DrugRequest());
        }

        // ================= ADD ADVICE =================
        [HttpGet]
        public IActionResult AddAdvice(int id)
        {
            var model = new PhysicianAdvice
            {
                ScheduleId = id,
                PhysicianPrescrips = new List<PhysicianPrescrip>
                {
                    new PhysicianPrescrip()
                }
            };

            ViewBag.drugs = new SelectList(
                _context.Drugs.Where(d => d.DrugStatus == "Active"),
                "DrugId",
                "DrugTitle"
            );

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAdvice(PhysicianAdvice model)
        {
            // 🔥 IMPORTANT: remove blocking validation
            // because your UI model != DB model perfectly
            ModelState.Clear();

            var advice = new PhysicianAdvice
            {
                ScheduleId = model.ScheduleId,
                Advice = model.Advice,
                Note = model.Note,
                PhysicianPrescrips = new List<PhysicianPrescrip>()
            };

            if (model.PhysicianPrescrips != null)
            {
                foreach (var p in model.PhysicianPrescrips)
                {
                    advice.PhysicianPrescrips.Add(new PhysicianPrescrip
                    {
                        DrugId = p.DrugId,
                        Prescription = p.Prescription,
                        Dosage = p.Dosage
                    });
                }
            }

            var schedule = _context.Schedules.Find(model.ScheduleId);

            if (schedule != null)
                schedule.ScheduleStatus = "Completed";

            _context.PhysicianAdvices.Add(advice);
            _context.SaveChanges();

            return RedirectToAction("ViewSchedule");
        }

        // ================= VIEW ADVICE =================
        public IActionResult ViewAdvice(int id)
        {
            var advice = _context.PhysicianAdvices
                .Include(a => a.Schedule)
                    .ThenInclude(s => s.Appointment)
                        .ThenInclude(a => a.Patient)
                .Include(a => a.PhysicianPrescrips)
                    .ThenInclude(d => d.Drug)
                .FirstOrDefault(a => a.ScheduleId == id);

            return View(advice);
        }
        
    }
}