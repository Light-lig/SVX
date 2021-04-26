using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SVX.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        public ActionResult ProductDetails()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Registrarme()
        {
            return View();
        }

        public ActionResult AddProduct()
        {
            return View("~/Views/Home/Anuncios/AddProduct.cshtml");
        }

        public ActionResult MisAnuncios()
        {
            return View("~/Views/Home/Anuncios/MisAnuncios.cshtml");
        }

        public ActionResult MiPerfil()
        {
            return View();
        }

        public ActionResult EditProduct()
        {
            return View("~/Views/Home/Anuncios/EditProduct.cshtml");
        }
        public ActionResult Chat()
        {
            return View();
        }
    }
}