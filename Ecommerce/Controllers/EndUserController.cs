using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class EndUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ShopViewing()
        {
            return View();
        }
    }
}
