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
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Mensaje
    {
        public int idMensaje { get; set; }
        public int idTo { get; set; }
        public int idFrom { get; set; }
        public int idConversacion { get; set; }
        public string mensaje1 { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public Nullable<System.DateTime> fecha { get; set; }
        [JsonIgnore]
        public virtual Conversacion Conversacion { get; set; }

        public virtual Usuario Usuario { get; set; }
    
        public virtual Usuario Usuario1 { get; set; }
    }
}
