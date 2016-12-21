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
    
    public partial class MOUVEMENTS
    {
        public MOUVEMENTS()
        {
            this.DETAILS_MOUVEMENTS = new HashSet<DETAILS_MOUVEMENTS>();
        }
    
        public int ID { get; set; }
        public string TYPE { get; set; }
        public System.DateTime DATE_MOUVEMENT { get; set; }
        public double DISTANCE { get; set; }
        public Nullable<int> DU { get; set; }
        public Nullable<int> AU { get; set; }
        public Nullable<int> CLIENT { get; set; }
        public Nullable<int> TRANSPORTEUR { get; set; }
    
        public virtual ICollection<DETAILS_MOUVEMENTS> DETAILS_MOUVEMENTS { get; set; }
        public virtual LIEUX DEPARTS { get; set; }
        public virtual LIEUX RETOURS { get; set; }
        public virtual TIERS CLIENTS { get; set; }
        public virtual TIERS TRANSPORTEURS { get; set; }
    }
}
