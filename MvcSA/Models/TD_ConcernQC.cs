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
    
    public partial class TD_ConcernQC
    {
        public int id { get; set; }
        public int group_id { get; set; }
    
        public virtual TD_Main TD_Main { get; set; }
    }
}
