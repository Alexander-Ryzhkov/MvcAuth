using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MvcAuth.Controllers
{
    public class SignInController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult vkontakte(string code = "", string state = "")
        {
            return View();
        }

        public IActionResult facebook(string code = "", string state = "")
        {
            return View();
        }

        public IActionResult twitter(string code = "", string state = "")
        {
            return View();
        }

    }
}
