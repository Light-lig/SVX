using SVX.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SVX.Controllers
{
    public class HomeController : Controller
    {
        ProyectoWeb2021Entities contexto = new ProyectoWeb2021Entities();
        Util objUtil = new Util();
        public ActionResult Index()
        {
            /*Usuario us = (Usuario)Session["idUsuario"];
            var productos = (from a in contexto.Anuncio
                             join u in contexto.Usuario on a.idUsuario equals u.idUsuario 
                             where ((u.idDepartamento == ((us==null)?null:us.idDepartamento)) || (us == null)) select a).ToList();*/
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
            ViewBag.Categoria = contexto.Categoria.Where(x => x.idSuper != null)
                .OrderBy(x=>x.nombre).Select(x => new SelectListItem { Text = x.nombre, Value = x.idCategoria.ToString()});
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddProduct(Anuncio ano)
        {
            ViewBag.Categoria = contexto.Categoria.Where(x => x.idSuper != null)
                .OrderBy(x => x.nombre).Select(x => new SelectListItem { Text = x.nombre, Value = x.idCategoria.ToString() });

            ano.idAnuncio = objUtil.GenerarCodigo("A");
            ano.estado = 1;
            ano.disponible = 1;
            ano.fecha = DateTime.Now;
            if (ano.idCategoria == 0)
                ano.idCategoria = 19;
            //sirve para ver el request osea toda la mierda que trae el form
            //var aux = Request.QueryString.AllKeys;
            if (ModelState.IsValid)
            {

                contexto.Anuncio.Add(ano);
                contexto.SaveChanges();
                //funciona con input
                //IList<HttpPostedFileBase> files = Request.Files.GetMultiple("files");
                const int maxFileLength = 5242880; // 5MB = 1024 * 5 * 1024
                Random r = new Random();
                foreach (HttpPostedFileBase file in ano.files)
                {
                    if (file != null && (file.ContentLength > 0 && file.ContentLength <= maxFileLength))
                    {
                        string path = HttpContext.Server.MapPath(@"~/archivos");
                        bool exists = System.IO.Directory.Exists(path);
                        string newName = DateTime.Now.ToString("yyyyMMddHHmmss");
                        int rand = r.Next();
                        if (!exists)
                            System.IO.Directory.CreateDirectory(path);
                        string extension = Path.GetExtension(file.FileName);
                        bool stateExt = false;
                        if (ValidateExtension(extension))
                            stateExt = true;
                        else
                            TempData["MessageError"] = "Solo .jpg, .png y .jpeg";
                        if (stateExt)
                        {
                            Foto ft = new Foto();
                            string newFile = newName + "-" + rand + extension;
                            ft.idFoto = objUtil.GenerarCodigo("F");
                            ft.idAnuncio = ano.idAnuncio;
                            ft.ruta = newFile;
                            contexto.Foto.Add(ft);
                            contexto.SaveChanges();
                            var ServerSavePath = Path.Combine(path, newFile);
                            file.SaveAs(ServerSavePath);
                        }
                    }
                    else
                        TempData["MessageError"] = "Archivo no debe ser mayor que 5mb.";
                }
                TempData["Message"] = "Registro Guardado con sus " + ano.files.Count().ToString() + " imagenes";
                return RedirectToAction("AddProduct");
            }
            else
                return View(ano);
        }
        private bool ValidateExtension(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return true;
                case ".png":
                    return true;
                case ".jpeg":
                    return true;
                default:
                    return false;
            }
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
            List<Categoria> datos = contexto.Categoria.ToList();
            ViewBag.categorias = datos;
            return PartialView("_Categories");
        }

        [HttpPost]
        public ActionResult Login(Usuario us)
        {
            Usuario user = contexto.Usuario.Where(u => u.email.Equals(us.email) && u.pass.Equals(us.pass)).FirstOrDefault();
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