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
        public int IdAnuncio { get; set; }
        
        [MinLength(length: 4, ErrorMessage = "Al menos 4 caracteres")]
        [MaxLength(length: 35, ErrorMessage = "No más de 35 caracteres")]
        [Display(Name = "Titulo")]
        public string Titulo { get; set; }
        
        [MinLength(length: 4, ErrorMessage = "Al menos 4 caracteres")]
        [MaxLength(length: 50, ErrorMessage = "No más de 50 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        
        [MinLength(length: 4, ErrorMessage = "Al menos 4 caracteres")]
        [MaxLength(length: 150, ErrorMessage = "No más de 150 caracteres")]
        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }

        [MinLength(length: 3, ErrorMessage = "Al menos 3 caracteres")]
        [MaxLength(length: 50, ErrorMessage = "No más de 50 caracteres")]
        [Display(Name = "Modelo")]
        public string Modelo { get; set; }
        
        [MinLength(length: 3, ErrorMessage = "Al menos 3 caracteres")]
        [MaxLength(length: 35, ErrorMessage = "No más de 35 caracteres")]
        [Display(Name = "Marca")]
        public string Marca { get; set; }
        
        [Display(Name = "Categoria")]
        public int IdCategoria { get; set; }

        public int IdUsuario { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }
        public Nullable<int> Disponible { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public Nullable<int> Estado { get; set; }
    }
}