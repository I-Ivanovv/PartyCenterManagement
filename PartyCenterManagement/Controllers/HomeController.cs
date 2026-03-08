using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartyCenterManagement.Models;
using System.Diagnostics;
using PartyCenterManagement.Services;

namespace PartyCenterManagement.Controllers
{
    public class HomeController : Controller
    {
        PackageServices _packageServices;
        public HomeController(PackageServices packageService) 
        {
            _packageServices = packageService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Packages()
        {
            var packages = await _packageServices.GetPackages();
            ViewBag.Services = await _packageServices.GetServices();
            return View(packages);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
