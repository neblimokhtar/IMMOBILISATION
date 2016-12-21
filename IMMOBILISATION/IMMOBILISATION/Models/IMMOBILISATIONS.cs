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
    
    public partial class IMMOBILISATIONS
    {
        public IMMOBILISATIONS()
        {
            this.FICHES_TECHNIQUES = new HashSet<FICHES_TECHNIQUES>();
            this.DETAILS_MOUVEMENTS = new HashSet<DETAILS_MOUVEMENTS>();
        }
    
        public int ID { get; set; }
        public string TYPE { get; set; }
        public string CODE { get; set; }
        public string DESIGNATION { get; set; }
        public double VALEUR_ACQUISITION_TTC { get; set; }
        public System.DateTime DATE_AQUISITION { get; set; }
        public System.DateTime DATE_MISE_EN_SERVICE { get; set; }
        public Nullable<int> FOURNISSEUR { get; set; }
        public Nullable<int> FAMILLE { get; set; }
        public string CODE_A_BARRE { get; set; }
        public bool DISPONIBILITE { get; set; }
    
        public virtual ICollection<FICHES_TECHNIQUES> FICHES_TECHNIQUES { get; set; }
        public virtual FAMILLES_IMMOBILISATIONS FAMILLES_IMMOBILISATIONS { get; set; }
        public virtual TIERS TIERS { get; set; }
        public virtual ICollection<DETAILS_MOUVEMENTS> DETAILS_MOUVEMENTS { get; set; }
    }
}
