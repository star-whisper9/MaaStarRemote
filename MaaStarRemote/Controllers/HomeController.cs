using MaaStarRemote.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MaaStarRemote.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var dropdownItems = _configuration.GetSection("ValidTasks").Get<List<string>>();
            ViewBag.DropdownItems = dropdownItems;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}