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
    
    public partial class Foto
    {
        public string idFoto { get; set; }
        public string ruta { get; set; }
        public string idAnuncio { get; set; }
    
        public virtual Anuncio Anuncio { get; set; }
    }
}
