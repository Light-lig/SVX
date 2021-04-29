using SVX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SVX.Controllers
{
    public class HomeController : Controller
    {
        ProyectoWeb2021Entities1 conexto = new ProyectoWeb2021Entities1();
        public ActionResult Index()
        {
            Usuario us = (Usuario)Session["idUsuario"];
            var productos = (from a in conexto.Anuncio
                             join u in conexto.Usuario on a.idUsuario equals u.idUsuario 
                             where ((u.idDepto == ((us==null)?null:us.idDepto)) || (us == null)) select a).ToList();
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
        [ChildActionOnly]
        public ActionResult RenderCategories()
        {
            List<Categoria> datos = conexto.Categoria.ToList();
            ViewBag.categorias = datos;
            return PartialView("_Categories");
        }

        [HttpPost]
        public ActionResult Login(Usuario us)
        {
            Usuario user = conexto.Usuario.Where(u => u.email.Equals(us.email) && u.contrasenia.Equals(us.contrasenia)).FirstOrDefault();
            if (user == null)
            {
                return View();
            }
            else
            {
                Session["idUsuario"] = user;
                return RedirectToAction("Index");
            }
        }
    }
}