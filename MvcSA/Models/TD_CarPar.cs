//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcSA.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TD_CarPar
    {
        public int id { get; set; }
        public string par_no { get; set; }
        public string car_no { get; set; }
        public string root_cause_section { get; set; }
        public string issue_by { get; set; }
        public Nullable<System.DateTime> create_dt { get; set; }
    
        public virtual TD_Main TD_Main { get; set; }
    }
}
