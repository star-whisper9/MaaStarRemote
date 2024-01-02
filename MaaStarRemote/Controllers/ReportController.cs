using Microsoft.AspNetCore.Mvc;

namespace MaaStarRemote.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
