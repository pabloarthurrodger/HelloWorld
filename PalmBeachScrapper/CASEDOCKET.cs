//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PalmBeachScrapper
{
    using System;
    using System.Collections.Generic;
    
    public partial class CASEDOCKET
    {
        public int ID { get; set; }
        public System.Guid CASEID { get; set; }
        public string COUNTY { get; set; }
        public string CASENUMBER { get; set; }
        public Nullable<System.DateTime> DOCKETDATE { get; set; }
        public string DOCKETDESCRIPTION { get; set; }
        public string PAGES { get; set; }
        public string IMAGEPATH { get; set; }
    
        public virtual MAINCAS MAINCAS { get; set; }
    }
}
