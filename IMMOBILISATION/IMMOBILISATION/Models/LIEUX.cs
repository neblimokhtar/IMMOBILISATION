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
    using System.ComponentModel;

    public partial class LIEUX
    {
        public LIEUX()
        {
            this.MOUVEMENTS = new HashSet<MOUVEMENTS>();
            this.MOUVEMENTS1 = new HashSet<MOUVEMENTS>();
        }

        public int ID { get; set; }
        public string INTITULE { get; set; }
        public string ADRESSE { get; set; }
        public string VILLE { get; set; }
        public string REGION { get; set; }
        public string PAYS { get; set; }
        public string TELEPHONE { get; set; }
        [DefaultValue(0)]
        public double LATITUDE { get; set; }
        [DefaultValue(0)]
        public double LONGITUDE { get; set; }
        public string CODE_POSTAL { get; set; }

        public virtual ICollection<MOUVEMENTS> MOUVEMENTS { get; set; }
        public virtual ICollection<MOUVEMENTS> MOUVEMENTS1 { get; set; }
    }
}