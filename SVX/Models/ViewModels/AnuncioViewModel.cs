using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SVX.Models.ViewModels
{
    public class AnuncioViewModel
    {
    }

    public class EditAnuncioViewModel
    {
        public string IdAnuncio { get; set; }
        [Required]
        [MinLength(length: 4, ErrorMessage = "Al menos 4 caracteres")]
        [MaxLength(length: 35, ErrorMessage = "No más de 35 caracteres")]
        [Display(Name = "Titulo")]
        public string Titulo { get; set; }
        [Required]
        [MinLength(length: 4, ErrorMessage = "Al menos 4 caracteres")]
        [MaxLength(length: 50, ErrorMessage = "No más de 50 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Required]
        [MinLength(length: 4, ErrorMessage = "Al menos 4 caracteres")]
        [MaxLength(length: 150, ErrorMessage = "No más de 150 caracteres")]
        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }
        [Required]
        [MinLength(length: 3, ErrorMessage = "Al menos 3 caracteres")]
        [MaxLength(length: 50, ErrorMessage = "No más de 50 caracteres")]
        [Display(Name = "Modelo")]
        public string Modelo { get; set; }
        [Required]
        [MinLength(length: 3, ErrorMessage = "Al menos 3 caracteres")]
        [MaxLength(length: 35, ErrorMessage = "No más de 35 caracteres")]
        [Display(Name = "Marca")]
        public string Marca { get; set; }
        [Required]
        [Display(Name = "Categoria")]
        public int IdCategoria { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }
        public Nullable<int> Disponible { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        [Required]
        public Nullable<int> Estado { get; set; }
        public HttpPostedFileBase[] files { get; set; }
    }
}