using SVX.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SVX.Hubs;
namespace SVX.Controllers
{
    public class HomeController : Controller
    {
        ProyectoWeb2021Entities contexto = new ProyectoWeb2021Entities();
        Util objUtil = new Util();

        public ActionResult Index(int id = 0, int idDepartamento = 0,  string filtro = "", int limit = 25)
        {
            Usuario us = (Usuario)Session["Usuario"];
            int idDepto = 0;
            if(us != null)
            {
                idDepto = us.idDepartamento;
            }
            var resultados = (from a in contexto.Anuncio
                             join u in contexto.Usuario on a.idUsuario equals u.idUsuario 
                             where  ((u.idDepartamento.Equals(idDepartamento)||(idDepartamento.Equals(0))) &&
                                    ((a.idCategoria.Equals(id)) || (id == 0)) &&
                                    ((a.titulo.Contains(filtro)) || (filtro == "")))
                                    select a).ToList();
            int limiteFinal = 0;
            if (resultados.Count < limit)
            {
                limiteFinal = resultados.Count;
            }
            else
            {
                limiteFinal = limit;
            }

            var productos = resultados.Take(limiteFinal).ToList();

            /*((u.idDepto.Equals(idDepto)) || (idDepto == 0)) &&*/

            var departamentos = contexto.Departamento.ToList();
            ViewBag.resultados = resultados.Count();
            ViewBag.departamentos = departamentos;
            ViewBag.productos = productos;
            ViewBag.limite = limit;
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

            var conversacion = (from m in contexto.Mensaje 
                                join u in contexto.Usuario on  m.idTo equals u.idUsuario
                                join a in contexto.Anuncio on  u.idUsuario equals a.idUsuario
                                where (m.idTo == producto.idUsuario) && (m.idFrom.Equals(idUser))|| (idUser.Equals(0)) &&
                                       (a.idAnuncio.Equals(id)) 
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
            Usuario us = (Usuario)Session["Usuario"];
            if (us != null)
            {
                ViewBag.Categoria = contexto.Categoria.Where(x => x.idSuper != null)
                 .OrderBy(x => x.nombre).Select(x => new SelectListItem { Text = x.nombre, Value = x.idCategoria.ToString() });
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }

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
        #region apartado chat



        public ActionResult Chat(int idUser = 0, int idConver = 0)
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
            Usuario user = contexto.Usuario.Where(u => u.email.Equals(us.email) && u.pass.Equals(us.pass)).FirstOrDefault();
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

        public ActionResult CerrarSession()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}