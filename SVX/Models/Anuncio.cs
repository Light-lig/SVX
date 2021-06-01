//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SVX.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    public partial class Anuncio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Anuncio()
        {
            this.Foto = new HashSet<Foto>();
        }
        [DataType(DataType.Upload)]
        [Display(Name = "Subir Archivo")]
        [Required(ErrorMessage = "Por favor seleccione un archivo.")]
        public HttpPostedFileBase[] files { get; set; }
        public int idAnuncio { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        [MinLength(length: 4, ErrorMessage = "Al menos 4 caracteres")]
        [MaxLength(length: 35, ErrorMessage = "No más de 35 caracteres")]
        [Display(Name = "Titulo")]
        public string titulo { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        [MinLength(length: 4, ErrorMessage = "Al menos 4 caracteres")]
        [MaxLength(length: 50, ErrorMessage = "No más de 50 caracteres")]
        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        [MinLength(length: 4, ErrorMessage = "Al menos 4 caracteres")]
        [MaxLength(length: 150, ErrorMessage = "No más de 150 caracteres")]
        [Display(Name = "Descripcion")]
        public string descripcion { get; set; }


        [Required(ErrorMessage = "Campo Requerido")]
        [MinLength(length: 3, ErrorMessage = "Al menos 3 caracteres")]
        [MaxLength(length: 50, ErrorMessage = "No más de 50 caracteres")]
        [Display(Name = "Modelo")]
        public string modelo { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        [MinLength(length: 3, ErrorMessage = "Al menos 3 caracteres")]
        [MaxLength(length: 35, ErrorMessage = "No más de 35 caracteres")]
        [Display(Name = "Marca")]
        public string marca { get; set; }
        
        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Categoria")]
        public int idCategoria { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        public int idUsuario { get; set; }


        [Required(ErrorMessage = "Campo Requerido")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio")]
        public decimal precio { get; set; }
        public Nullable<int> disponible { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<int> estado { get; set; }


        public virtual Categoria Categoria { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Foto> Foto { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
