﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SvxEntities : DbContext
    {
        public SvxEntities()
            : base("name=SvxEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Anuncio> Anuncio { get; set; }
        public virtual DbSet<Categoria> Categoria { get; set; }
        public virtual DbSet<Conversacion> Conversacion { get; set; }
        public virtual DbSet<Departamento> Departamento { get; set; }
        public virtual DbSet<Foto> Foto { get; set; }
        public virtual DbSet<mensaje> mensaje { get; set; }
        public virtual DbSet<puntuacion> puntuacion { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
    }
}