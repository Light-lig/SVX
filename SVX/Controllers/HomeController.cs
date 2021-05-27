using SVX.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SVX.Hubs;
namespace SVX.Controllers
{
    public class HomeController : Controller
    {
        SvxEntities contexto = new SvxEntities();
        public ActionResult Index(int id = 0)
        {
            Usuario us = (Usuario)Session["Usuario"];
            int idDepto = 0;
            int idUser = 0;
            if(us != null)
            {
                idDepto = us.idDepto;
                idUser = us.idUsuario;
            }
            var productos = (from a in contexto.Anuncio
                             join u in contexto.Usuario on a.idUsuario equals u.idUsuario 
                             where /*((u.idDepto.Equals(idDepto)) || (idDepto == 0)) &&*/
                                    ((a.idCategoria.Equals(id)) || (id == 0)) &&
                                    ((a.idUsuario != idUser) || (idUser == 0)) select a).ToList();
            ViewBag.productos = productos;
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
        
        public ActionResult ProductDetails(int id)
        {
            Usuario us = (Usuario)Session["Usuario"];
            int idUser = 0;
            if(us != null)
            {
                idUser = us.idUsuario;
            }
            var producto = contexto.Anuncio.Where(a => a.idAnuncio.Equals(id)).FirstOrDefault();

            var conversacion = (from c in contexto.Conversacion
                                join m in contexto.mensaje on c.idConver equals m.idConversacion
                                join u in contexto.Usuario on  m.idTo equals u.idUsuario
                                join a in contexto.Anuncio on  u.idUsuario equals a.idUsuario
                                where ((m.idFrom.Equals(idUser)) || (idUser == 0)) &&
                                       ((a.idAnuncio.Equals(id)))
                                select m).FirstOrDefault();

            var productosRelacionados = contexto.Anuncio.ToList().Take(4);
            ViewBag.producto = producto;
            ViewBag.productosRelacionados = productosRelacionados;
            ViewBag.conversacion = conversacion;
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Registrarme()
        {
            var departamentos = contexto.Departamento.ToList();
            ViewBag.Departamentos = departamentos;
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
        #region apartado chat



        public ActionResult Chat(int idUser, int idConver)
        {
            Usuario us = (Usuario)Session["Usuario"];
            if(us != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
           
        }
        #endregion
        [ChildActionOnly]
        public ActionResult RenderCategories()
        {
            List<Categoria> datos = contexto.Categoria.ToList();
            ViewBag.categorias = datos;
            return PartialView("_Categories");
        }

        [HttpPost]
        public ActionResult Login(Usuario us)
        {
            Usuario user = contexto.Usuario.Where(u => u.email.Equals(us.email) && u.contrasenia.Equals(us.contrasenia)).FirstOrDefault();
            if (user == null)
            {
                return Json(new { result = false });
            }
            else
            {
                Session["Usuario"] = user;
                return Json(new { result = "/Home/Index"});
            }
        }

        [HttpPost]
        public ActionResult Registrarme(Usuario us)
        {
            var usuarioExistente = contexto.Usuario.Where(u => u.email.Equals(us.email)).FirstOrDefault(); 
            if(usuarioExistente != null)
            {
                return Json(new { result = false, mensaje = "El correo especificado ya esta asociado a una cuenta, favor utilizar otro correo." });
            }
            else
            {
                contexto.Usuario.Add(us);
                contexto.SaveChanges();
                var usuario = contexto.Usuario.ToList().Last();
                Session["Usuario"] = usuario;
                return Json(new { result = "/Home/Index", mensaje = "Bien hecho, Bienvenido a nuestro sitio." });
            }

        }
    }
}