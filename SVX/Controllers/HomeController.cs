using SVX.Models;
using SVX.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

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
                                    a.disponible == 1 &&
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
        public ActionResult ProductDetails(string id)
        {
            if(id != null)
            {
                Usuario us = (Usuario)Session["Usuario"];
                int idUser = 0;
                if (us != null)
                {
                    idUser = us.idUsuario;
                }
                var producto = contexto.Anuncio.Where(a => a.idAnuncio.Equals(id)).FirstOrDefault();

                var conversacion = (from m in contexto.Mensaje
                                    join u in contexto.Usuario on m.idTo equals u.idUsuario
                                    join a in contexto.Anuncio on u.idUsuario equals a.idUsuario
                                    where (m.idTo == producto.idUsuario) && (m.idFrom.Equals(idUser)) || (idUser.Equals(0)) &&
                                           (a.idAnuncio.Equals(id))
                                    select m).FirstOrDefault();
                var puntuacion = contexto.Puntuacion.Where(p => p.idUsuario == producto.idUsuario).ToList();
                double? rating = 0.0;
                if (puntuacion.Count > 0)
                {
                    rating = puntuacion.Sum(p => p.valor) / puntuacion.Count;
                }

                var productosRelacionados = contexto.Anuncio.ToList().Take(4);
                ViewBag.producto = producto;
                ViewBag.rating = rating;
                ViewBag.productosRelacionados = productosRelacionados;
                ViewBag.conversacion = conversacion;
                return View();
            }
            else
            {
                return RedirectToAction("Error404");
            }
        
        }
        [HttpPost]
        public ActionResult rate(Puntuacion pun)
        {
            var estado = true;
            var mensaje = "";
            try
            {
                Usuario us = (Usuario)Session["Usuario"];
                if (us != null)
                {
                    Puntuacion rating = new Puntuacion();
                    var puntaciones = contexto.Puntuacion.Where(p => p.idUsuario == pun.idUsuario && p.idUsuarioPuntua == pun.idUsuarioPuntua).ToList().FirstOrDefault();
                    if (puntaciones == null)
                    {
                        mensaje = "Se puntuo correctamente.";
                        rating.idUsuario = pun.idUsuario;
                        rating.idUsuarioPuntua = pun.idUsuarioPuntua;
                        rating.valor = pun.valor;
                        contexto.Puntuacion.Add(rating);
                        contexto.SaveChanges();
                    }
                    else
                    {
                        mensaje = "Se puntuo correctamente.";
                        puntaciones.valor = pun.valor;
                        contexto.SaveChanges();
                    }
                }
                else
                {
                    estado = false;
                    mensaje = "Debe iniciar sesion para poder realizar esta accion.";
                }
               
            }
            catch (Exception e)
            {
                estado = false;
                mensaje = "Ocurrio un error: " + e.Message.ToString();
            }

           
            return Json(new { result = estado, mensaje = mensaje});
        }
        public ActionResult Login()
        {
            if(TempData["mensaje"] != null)
            {
                ViewBag.mensaje = TempData["mensaje"].ToString();
            }
            return View();
        }
        public ActionResult RecuperarContrasenia()
        {
            return View();
        }
        public ActionResult CambiarContrasenia(string id)
        {
            if(id != null)
            {
                ViewBag.idUsuario = Base64Decode(id);
                return View();
            }
            else
            {
                return RedirectToAction("Error404");
            }
       
        }
        [HttpPost]
        public ActionResult CambiarContrasenia(Usuario us)
         {
            var usuario = contexto.Usuario.Where(u => u.idUsuario == us.idUsuario).FirstOrDefault();
            try
            {
                usuario.pass = Base64Encode(us.pass);
                contexto.SaveChanges();
                Session["Usuario"] = null;
                return Json(new { result = "/Home/Login", mensaje = "Se cambio la contrasenia correctamente." });
            }
            catch (Exception e)
            {
                return Json(new { result = false, mensaje = "Ocurrio un problema interno" });

            }
        }
        [HttpPost]
        public ActionResult EnviarCorreoCambiarContra(string email)
        {
            try
            {

                var usuario = contexto.Usuario.Where(u => u.email.Equals(email)).FirstOrDefault();
                if(usuario != null)
                {
                    string mensaje = "<h1>SVX</h1><br/>" +
                         "<center><p>Favor presione el siguiente link para cambiar su contra.</p></center><br/>" +
                         "<a href='https://localhost:44369/Home/CambiarContrasenia/" + Base64Encode(usuario.idUsuario.ToString()) + "'>Cambiar contra</a>";
                    if (sendEmail(email, "Recuperar contrasenia", mensaje))
                    {
                        return Json(new { result = true, mensaje = "Se envio un mensaje a su correo para poder restablecer la contrasenia." });
                    }
                    else
                    {
                        return Json(new { result = false, mensaje = "Ocurrio un problema al momento de enviar su correo." });
                    }
                }
                else
                {
                    return Json(new { result = false, mensaje = "El email que ha ingresado no esta asociado a ninguna cuenta." });
                }
            }
                
            catch (Exception e)
            {

                return Json(new { result = false, mensaje ="Ocurrio un problema interno." });
            }
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
                ViewBag.Message = TempData["Message"];
                ViewBag.MessageError = TempData["MessageError"];
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
            try
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
                    if(ano.files != null)
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
                  
                  
                    }

                    TempData["Message"] = "Registro Guardado";
                    return RedirectToAction("AddProduct");
                }
                else
                    return View(ano);
            }
            catch (Exception e)
            {

                return RedirectToAction("Error500");
            }
          
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
            Usuario us = (Usuario)Session["Usuario"];
            if (us != null)
            {
                if (TempData.ContainsKey("Message"))
                {
                    ViewBag.Message = TempData["Message"].ToString();
                }
                return View("~/Views/Home/Anuncios/MisAnuncios.cshtml");
            }
            else
            {
                return RedirectToAction("Login");
            }            
        }
        public ActionResult MiPerfil()
        {
            Usuario us = (Usuario)Session["Usuario"];
            if (us != null)
            {
                var departamentos = contexto.Departamento.ToList();
                ViewBag.Departamentos = departamentos;
                return View(us);
            }
            else
            {
                return RedirectToAction("Login");
            }
          
        }
        [HttpPost]
        public ActionResult ActualizarPerfil(Usuario us)
        {
            try
            {
                var usuario = contexto.Usuario.Where(u => u.idUsuario == us.idUsuario).FirstOrDefault();
                usuario.nombre = us.nombre;
                usuario.apellido = us.apellido;
                if(usuario.email != us.email)
                {
                    var email = contexto.Usuario.Where(u => u.email == us.email).FirstOrDefault();
                    if(email != null)
                    {
                        return Json(new { result = false, mensaje ="El correo ya esta asociado a un cuenta." });
                    }
                    else
                    {
                        usuario.email = us.email;

                    }
                }
                usuario.telefono = us.telefono;
                usuario.idDepartamento = us.idDepartamento;
                contexto.SaveChanges();
                Session["Usuario"] = usuario;
                return Json(new { result = true, mensaje = "Cambios aplicados correctamente." });

            }
            catch (Exception e)
            {

                return Json(new { result = false, mensaje = "Ocurrio un error al procesar los datos." });
            }

        }
        public ActionResult EditProduct()
        {
            Usuario us = (Usuario)Session["Usuario"];
            if (us != null)
            {
                return View("~/Views/Home/Anuncios/EditProduct.cshtml");
            }
            else
            {
                return RedirectToAction("Login");
            }            
        }
        #region apartado chat

        public ActionResult Chat(int? idUser = 0, int? idConver = 0)
        {
            if(idUser != null && idConver != null)
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
            else
            {
                return RedirectToAction("Error404");
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
            Usuario user = contexto.Usuario.Where(u => u.email.Equals(us.email) && u.estado == 1).FirstOrDefault();
            if (user == null)
            {
                return Json(new { result = false });
            }
            else
            {
                if (Base64Decode(user.pass).Equals(us.pass))
                {
                    Session["Usuario"] = user;
                    return Json(new { result = "/Home/Index" });
                }
                else
                {
                    return Json(new { result = false });
                }

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
              
                us.pass = Base64Encode(us.pass);
                contexto.Usuario.Add(us);
                contexto.SaveChanges();
                var usuario = contexto.Usuario.ToList().Last();
                Session["Usuario"] = usuario;
                string mensaje = "<h1> SVX </h1><br/>" +
                                  "<center><p>Favor presione el siguiente link para activar su cuenta.</p></center><br/>" +
                                  "<a href='https://localhost:44369/Home/ActivarCuenta/" + Base64Encode(usuario.idUsuario.ToString())+"'>Activar cuenta</a>";
                sendEmail(us.email,"Activar", mensaje);
                return Json(new { result = true, mensaje = "Bien hecho, si tu correo es real, deberia haber haberte caido un correo para activar tu cuenta." });
            }
        }
        public ActionResult ActivarCuenta(string id)
        {
            try
            {
                if(id != null)
                {
                    int idFinal = int.Parse(Base64Decode(id));
                    var usuario = contexto.Usuario.Where(u => u.idUsuario == idFinal).FirstOrDefault();
                    usuario.estado = 1;
                    contexto.SaveChanges();
                    TempData["mensaje"] = "Su cuenta ha sido activada exitosamente.";
                    return RedirectToAction("Login");
                }
                else
                {
                    return RedirectToAction("Error404");
                }

            }
            catch (Exception e)
            {

                return RedirectToAction("Error500");
            }
          
        }
        public bool sendEmail(string receiver, string subject, string message)
        {
            try
            {
               
                    var senderEmail = new MailAddress("proyectospruebas390@gmail.com", "Proyectos Pruebas");
                    var receiverEmail = new MailAddress(receiver, "Receiver");
                    var password = "Manzana10";
                    var sub = subject;
                    var body = message;
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };

                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                    })
                    {
                        smtp.Send(mess);
                    }
                    return true;
            
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        public static string Base64Encode(string word)
        {
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(word);
            return Convert.ToBase64String(byt);
        }
        public static string Base64Decode(string word)
        {
            byte[] b = Convert.FromBase64String(word);
            return System.Text.Encoding.UTF8.GetString(b);
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
                Usuario us = (Usuario)Session["Usuario"];

                if (us != null)
                {
                    var query = from a in contexto.Anuncio
                                where a.idUsuario.Equals(us.idUsuario)
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

                        foreach (Foto item2 in imagen)
                        {
                            temp.foto = "<img src=\"../archivos/" + item2.ruta + "\" alt=\"" + item2.idFoto + "\" width=\"80\" onerror=\"this.src = '../images/404 Page Not Found _Outline.png'\">";
                        }

                        DateTime dateTime = (DateTime)item.fecha;
                        temp.fecha = dateTime.ToString("dddd, dd MMMM yyyy");

                        datosF.Add(temp);
                    }

                    return Json(new { data = datosF }, JsonRequestBehavior.AllowGet);
                }
                return Json(null);
                
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
            ViewBag.Categoria = contexto.Categoria.Where(x => x.idSuper != null)
                 .OrderBy(x => x.nombre).Select(x => new SelectListItem { Text = x.nombre, Value = x.idCategoria.ToString() });
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
                oAnuncio.idCategoria = model.IdCategoria;
              
                if(oAnuncio.files != null)
                {
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
                }
                

                TempData["Message"] = "Se actualizo con exito el anuncio.";
                contexto.Entry(oAnuncio).State = System.Data.Entity.EntityState.Modified;
                contexto.SaveChanges();
                return RedirectToAction("MisAnuncios");
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
                var delA = from a in contexto.Anuncio                          
                          where a.idAnuncio.Equals(id)
                          select new { idA = a};

                var delF = from f in contexto.Foto
                          where f.idAnuncio.Equals(id)
                          select new {idF = f };

                foreach (var item in delA)
                {
                    contexto.Anuncio.Remove(item.idA);                   
                    contexto.Entry(item.idA).State = System.Data.Entity.EntityState.Deleted;
                }

                foreach (var item in delF)
                {
                    contexto.Foto.Remove(item.idF);
                    borrarArchivo(item.idF.ruta);
                    contexto.Entry(item.idF).State = System.Data.Entity.EntityState.Deleted;
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
                borrarArchivo(f.ruta);                
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
        public void borrarArchivo(string nombreArchivo)
        {
            string sourcePath = HttpContext.Server.MapPath(@"~/archivos");
            string fileName = nombreArchivo;
            string url = "";
            string nameFile = "";

            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);

                foreach (string s in files)
                {
                    nameFile = System.IO.Path.GetFileName(s);
                    if (nameFile.Contains(fileName))
                    {
                        Console.Write("-> " + System.IO.Path.GetFileName(s) + "\n");
                        url = sourcePath + @"\" + nameFile;
                        Console.Write("-> " + url + "\n");
                        try
                        {
                            System.IO.File.Delete(url);
                            Console.Write("-> " + " Borrado" + "\n");
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine(e.Message);
                            return;
                        }
                    }

                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }
        }
        #endregion MisAnuncios
        public ActionResult CerrarSession()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
        public ActionResult Error404()
        {
            return View();
        }
        public ActionResult Error500()
        {
            return View();
        }
    }
}