﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IMMOBILISATION.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ImmobilisationEntities : DbContext
    {
        public ImmobilisationEntities()
            : base("name=ImmobilisationEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<FAMILLES_IMMOBILISATIONS> FAMILLES_IMMOBILISATIONS { get; set; }
        public DbSet<FICHES_TECHNIQUES> FICHES_TECHNIQUES { get; set; }
        public DbSet<LIEUX> LIEUX { get; set; }
        public DbSet<NATURES_BIENS> NATURES_BIENS { get; set; }
        public DbSet<TIERS> TIERS { get; set; }
        public DbSet<IMMOBILISATIONS> IMMOBILISATIONS { get; set; }
    }
}
