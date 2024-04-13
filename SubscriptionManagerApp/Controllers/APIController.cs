using Microsoft.AspNetCore.Mvc;

namespace SubscriptionManagerApp.Controllers
{
    public class APIController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
