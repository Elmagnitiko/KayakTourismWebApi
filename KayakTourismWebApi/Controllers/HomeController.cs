using KayakTourismWebApi.ControllersNS;
using KayakTourismWebApi.ModelsNS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KayakTourismWebApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<Customer> _userManager;

        public HomeController(UserManager<Customer> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                return RedirectToAction(nameof(EventsController.GetAll), "Todos");
            }
            ViewData["Title"] = "Home";
            return View();
        }

        public IActionResult About()
        {
            ViewData["Title"] = "About";
            return View();
        }
    }
}
