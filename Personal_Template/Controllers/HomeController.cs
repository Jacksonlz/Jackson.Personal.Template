using Microsoft.AspNetCore.Mvc;

namespace Personal.Template.Web.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
