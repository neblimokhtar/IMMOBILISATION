//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class DETAILS_MOUVEMENTS
    {
        public int ID { get; set; }
        public Nullable<int> MOUVEMENT { get; set; }
        public Nullable<int> IMMOBILISATION { get; set; }
        [ForeignKey("IMMOBILISATION")]
        public virtual IMMOBILISATIONS IMMOBILISATIONS { get; set; }
        [ForeignKey("MOUVEMENT")]
        public virtual MOUVEMENTS MOUVEMENTS { get; set; }
    }
}