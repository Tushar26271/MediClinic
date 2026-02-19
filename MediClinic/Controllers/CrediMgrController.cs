using MediClinic.Models.ViewModel;
using MediClinic.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace MediClinic.Controllers
{
    public class CrediMgrController : Controller
    {
        private readonly MediCureContext _context;
        public CrediMgrController(MediCureContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {

            MediClinic.Models.MediCureContext db =new MediClinic.Models.MediCureContext();

            var usr = db.Users.FirstOrDefault(u => u.UserName == username && u.Password == password && u.Status=="Active");



            if (usr != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, usr.Role),
                    new Claim("RoleReferenceId", usr.RoleReferenceId.ToString()),
                    new Claim("UserId", usr.UserId.ToString())
                };

                var identity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                return RedirectToAction("Index", usr.Role);
            }

            ModelState.AddModelError("", "Invalid credentials");

            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> AccessDenied()
        {
            
            return View();
        }
        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,PatientName,Dob,Gender,Address,Phone,Email,Summary")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                patient.PatientStatus = "Pending";
                _context.Add(patient);
                await _context.SaveChangesAsync();
                TempData["ShowSuccess"] = true;
                TempData["SuccessMessage"] =
                "Record created successfully. Your temporary password is your name followed by @ and the last four digits of your registered phone number.";
                return RedirectToAction("Index", "Home");
            }
            return View(patient);
        }
    }
}
