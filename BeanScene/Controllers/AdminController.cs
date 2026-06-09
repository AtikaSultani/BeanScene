using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeanScene.Controllers
{
    [Authorize(Roles = "Manager,Staff")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}