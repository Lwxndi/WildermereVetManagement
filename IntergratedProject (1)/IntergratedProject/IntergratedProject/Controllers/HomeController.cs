using IntergratedProject.Data;
using IntergratedProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using IntergratedProject.ViewModels;    
using IntergratedProject.Security;



namespace IntergratedProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IntergratedProjectDatabase _context;
        private readonly IEmailSender _emailSender;


        public HomeController(ILogger<HomeController> logger, IntergratedProjectDatabase context,IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // GET: /Home/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Home/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 🔍 Try PetOwner login
            var petOwner = await _context.PetOwner
                .FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password);
            if (petOwner != null)
            {
                // Set session variables
                HttpContext.Session.SetString("Role", petOwner.Role);
                HttpContext.Session.SetString("Email", petOwner.Email);
                HttpContext.Session.SetInt32("PetOwnerId", petOwner.OwnerId);

                // Redirect to PetOwner Dashboard
                return RedirectToAction("Dashboard", "PetOwners");
            }

            //  Try Veterinarian login
            var vet = await _context.Veterinarian
                .FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password);
            if (vet != null)
            {
                HttpContext.Session.SetString("Role", vet.Role);
                HttpContext.Session.SetString("Email", vet.Email);
                HttpContext.Session.SetInt32("VeterinarianId", vet.VeterinarianId); 

                // Redirect to Veterinarian Dashboard
                return RedirectToAction("Dashboard", "Veterinarians");
            }

            //  Try Admin login
            var admin = await _context.Admin
                .FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password);
            if (admin != null)
            {
                HttpContext.Session.SetString("Role", admin.Role);
                HttpContext.Session.SetString("Email", admin.Email);
                HttpContext.Session.SetInt32("AdminId", admin.AdminId); 

                // Redirect to Admin Dashboard
                return RedirectToAction("Dashboard", "Admins");
            }

            //  Invalid credentials
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }


        // GET: /Home/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Home/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.PetOwner
                .FirstOrDefaultAsync(p => p.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "No account found with that email.");
                return View(model);
            }

            // You'd normally generate a token and store it, but for simplicity:
            var resetLink = Url.Action("ResetPassword", "Home", new { email = model.Email }, Request.Scheme);

            await _emailSender.SendEmailAsync(model.Email, "Reset your password",
                $"Click <a href='{resetLink}'>here</a> to reset your password.");

            ViewBag.Message = "Password reset link has been sent to your email.";
            return View();
        }

        // GET: /Home/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            return View(new ResetPasswordViewModel { Email = email });
        }

        // POST: /Home/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.PetOwner.FirstOrDefaultAsync(p => p.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            user.Password = model.NewPassword; // Consider hashing in production
            _context.Update(user);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Password reset successfully.";
            return RedirectToAction("Login");
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }




    }
}
