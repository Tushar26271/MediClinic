using MediClinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;


namespace MediClinic.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly MediCureContext _context;
        public PatientController(MediCureContext context)
        {
            _context = context;
        }
        // HttpContext.Session.SetString("LastVisited")
        public async Task<IActionResult> Index()
        {
            int patientId = int.Parse(User.FindFirst("RoleReferenceId")!.Value);

            var patient = await _context.Patients
                                        .FirstOrDefaultAsync(p => p.PatientId == patientId);

            return View(patient);
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: DPatient/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,PatientName,Dob,Gender,Address,Phone,Email,Summary,PatientStatus")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }
        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }
        // GET: dummmointments/Create
        public IActionResult CreateAppointment()
        {
            return View();
        }


        // POST: dummmointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment(Appointment appointment)
        {
            int patientId = int.Parse(User.FindFirst("RoleReferenceId")!.Value);

            // set values manually
            appointment.PatientId = patientId;
            appointment.ScheduleStatus = "Pending";

            // VERY IMPORTANT FIX
            ModelState.Remove("Patient");
            ModelState.Remove("ScheduleStatus");

            if (ModelState.IsValid)
            {
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // back to dashboard
            }

            return View(appointment);
        }





        // GET: dummmointments/Details/5
        public async Task<IActionResult> MyAppointments(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var appointments = await _context.Appointments
                .Where(a => a.PatientId == id && a.ScheduleStatus == "Pending")
                .ToListAsync();

            return View(appointments);
        }

        public async Task<IActionResult> ViewScheduleAppointment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedules = await _context.Schedules
                .Include(s => s.Appointment)
                .Include(s => s.Physician)
                .Where(s => s.Appointment.PatientId == id
                            && s.ScheduleStatus == "Scheduled")
                .ToListAsync();

            if (schedules == null || !schedules.Any())
            {
                return NotFound();
            }

            return View(schedules);
        }
        public async Task<IActionResult> ViewAdvice(int id)
        {
            var adviceList = await _context.PhysicianAdvices
                .Where(a => a.ScheduleId == id)
                .ToListAsync();

            return View(adviceList);
        }


        public async Task<IActionResult> ViewPrescription(int id)
        {
            var prescription = await _context.PhysicianPrescrips
                .Where(p => p.PhysicianAdvice.ScheduleId == id)
                .ToListAsync();

            return View(prescription);
        }



    }
}













