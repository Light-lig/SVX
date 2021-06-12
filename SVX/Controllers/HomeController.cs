using SVX.Models;
using SVX.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

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
            if (us != null)
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
                //contexto.Anuncio.Add(ano);
                contexto.Entry(ano).State = System.Data.Entity.EntityState.Added;
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
            if (us != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        #endregion apartado chat

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
                return Json(new { result = "/Home/Index" });
            }
        }

        [HttpPost]
        public ActionResult Registrarme(Usuario us)
        {
            var usuarioExistente = contexto.Usuario.Where(u => u.email.Equals(us.email)).FirstOrDefault();
            if (usuarioExistente != null)
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

        #region MisAnuncios

        private struct anun_struct
        {
            public string id { get; set; }
            public string titulo { get; set; }
            public string nombre { get; set; }
            public string descripcion { get; set; }
            public string modelo { get; set; }
            public string marca { get; set; }
            public string precio { get; set; }
            public string disponible { get; set; }
            public string estado { get; set; }
            public string opc { get; set; }
            public string foto { get; set; }
            public string fecha { get; set; }
        }

        private struct Foto_struct
        {
            public string IdFoto { get; set; }
            public string Nombre { get; set; }
            public string IdAnuncio { get; set; }
        }

        [HttpGet]
        public JsonResult GetMisAnuncios()
        {
            try
            {
                var query = from a in contexto.Anuncio
                            select a;

                List<Anuncio> datos = query.ToList();
                List<anun_struct> datosF = new List<anun_struct>();
                int i = 0;
                foreach (Anuncio item in datos)
                {
                    i++;
                    anun_struct temp = new anun_struct();
                    temp.id = item.idAnuncio;
                    temp.titulo = item.titulo;
                    temp.nombre = item.nombre;
                    temp.descripcion = item.descripcion;
                    temp.marca = item.marca;
                    temp.modelo = item.modelo;
                    temp.precio = item.precio.ToString("C", new CultureInfo("en-US"));

                    if (item.disponible == 1)
                    {
                        temp.disponible = "<span class=\"label bg-green\" >Disponible</span>";
                        temp.opc = "<div class=\"btn-group\">" +
                            "<a role=\"button\" class=\"btn text-white btn-success\" id=\"btnMarcar\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Marcar como vendido\">" +
                        "<i class=\"fa fa-shopping-cart\"></i>" +
                        "</a>" +
                        "<a role=\"button\" class=\"btn text-white btn-info\" id=\"btnEditar\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Editar anuncio\">" +
                              "<i class=\"fa fa-edit\"></i>" +
                        "</a>" +
                        "<a role=\"button\" class=\"btn text-white btn-danger\" id=\"btnEliminar\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Eliminar anuncio\">" +
                              "<i class=\"fa fa-trash\"></i>" +
                        "</a>" +
                        "</div>";
                    }
                    else
                    {
                        temp.disponible = "<span class=\"label bg-secondary\" >No Disponible</span>";
                        temp.opc = "<a role=\"button\" class=\"btn btn-danger text-white\" id=\"btnEliminar\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Eliminar anuncio\">" +
                              "<i class=\"fa fa-trash\"></i>" +
                        "</a>";
                    }

                    if (item.estado == 1)
                    {
                        temp.estado = "<span class=\"label bg-blue\" >Nuevo</span>";
                    }
                    else
                    {
                        temp.estado = "<span class=\"label bg-warning\" >Usado</span>";
                    }

                    var imagen = (from f in contexto.Foto
                                  where f.idAnuncio.Equals(item.idAnuncio)
                                  orderby f.idFoto ascending
                                  select f).Take(1);
                    
                    foreach(Foto item2 in imagen)
                    {
                        temp.foto = "<img src=\"../archivos/"+ item2.ruta + "\" alt=\"" + item2.idFoto + "\" width=\"80\" onerror=\"this.src = '../images/404 Page Not Found _Outline.png'\">";
                    }

                    DateTime dateTime = (DateTime)item.fecha;
                    temp.fecha = dateTime.ToString("dddd, dd MMMM yyyy");

                    datosF.Add(temp);
                }
                
                return Json(new { data = datosF}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpGet]
        public ActionResult EditarAnuncio(string id)
        {
            Anuncio model = new();
            var oAnuncio = contexto.Anuncio.Find(id);
            model.idAnuncio = oAnuncio.idAnuncio;
            model.titulo = oAnuncio.titulo;
            model.nombre = oAnuncio.nombre;
            model.marca = oAnuncio.marca;
            model.modelo = oAnuncio.modelo;
            model.precio = oAnuncio.precio;
            model.idCategoria = oAnuncio.idCategoria;
            model.descripcion = oAnuncio.descripcion;
            model.estado = oAnuncio.estado;
            model.files = oAnuncio.files;
            model.disponible = oAnuncio.disponible;
            model.Foto = oAnuncio.Foto;
            model.Categoria = oAnuncio.Categoria;           
            
            return View("~/Views/Home/Anuncios/EditarAnuncio.cshtml", model);
        }


        [HttpPost]
        public ActionResult DbEditarAnuncio(EditAnuncioViewModel model)
        {
            try
            {
                var oAnuncio = contexto.Anuncio.Find(model.IdAnuncio);
                oAnuncio.titulo = model.Titulo;
                oAnuncio.nombre = model.Nombre;
                oAnuncio.modelo = model.Modelo;
                oAnuncio.marca = model.Marca;
                oAnuncio.precio = model.Precio;
                oAnuncio.descripcion = model.Descripcion;                
                oAnuncio.files = model.files;

                const int maxFileLength = 5242880;
                Random r = new Random();

                foreach (HttpPostedFileBase file in oAnuncio.files)
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
                            ft.idAnuncio = model.IdAnuncio;
                            ft.ruta = newFile;
                            contexto.Foto.Add(ft);
                            contexto.SaveChanges();
                            var ServerSavePath = Path.Combine(path, newFile);
                            file.SaveAs(ServerSavePath);
                        }
                    }
                }

                TempData["Message"] = "Se actualizo con exito el anuncio.";
                contexto.Entry(oAnuncio).State = System.Data.Entity.EntityState.Modified;
                contexto.SaveChanges();                
                return Json("ok");
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
                return Json("error al editar control");
            }
        }

        [HttpGet]
        public JsonResult EliminarAnuncio(string id)
        {
            try
            {                
                var del = from a in contexto.Anuncio
                          join f in contexto.Foto on a.idAnuncio equals f.idAnuncio
                          where a.idAnuncio.Equals(id)
                          select new { idA = a, idF = f };

                foreach(var item in del)
                {
                    contexto.Foto.Remove(item.idF);
                    contexto.Anuncio.Remove(item.idA);
                    contexto.Entry(item.idF).State = System.Data.Entity.EntityState.Deleted;
                    contexto.Entry(item.idA).State = System.Data.Entity.EntityState.Deleted;
                }
                
                contexto.SaveChanges();
                return Json("ok", JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }

                return Json("F");
            }
        }

        [HttpGet]
        public JsonResult CambiarDisponibilidad(string id)
        {
            try
            {
                var oAnuncio = contexto.Anuncio.Find(id);
                oAnuncio.disponible = 0;
                contexto.Entry(oAnuncio).State = System.Data.Entity.EntityState.Modified;
                contexto.SaveChanges();
                return Json("ok", JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }

                return Json("F");
            }
        }

        [HttpGet]
        public JsonResult CargarFotos(string id)
        {
            var imagen = (from f in contexto.Foto
                         where f.idAnuncio.Equals(id)
                         orderby f.idFoto ascending
                         select f).ToList();
            List<Foto> datos = new();
            List<Foto_struct> fotos = new();
            foreach (Foto item2 in imagen)
            {
                Foto_struct tmp2 = new();
                tmp2.IdAnuncio = item2.idAnuncio;
                tmp2.Nombre = item2.ruta;
                tmp2.IdFoto = item2.idFoto;
                fotos.Add(tmp2);
            }
            return Json(new { img = fotos }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarFotos(Foto f)
        {
            try
            {
                Foto deleteFoto = new Foto { idFoto = f.idFoto };                
                contexto.Entry(deleteFoto).State = System.Data.Entity.EntityState.Deleted;
                contexto.SaveChanges();
                return Json("ok");
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }

                return Json("F");
            }
        }

        #endregion MisAnuncios

        public ActionResult CerrarSession()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}