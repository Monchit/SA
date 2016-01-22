using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSA.ViewModels
{
    public class VM_TellIssuer
    {
        public int id { get; set; }
        public int tell_id { get; set; }
        public string tell_content { get; set; }
        public System.DateTime tell_dt { get; set; }
        public Nullable<System.DateTime> reply_dt { get; set; }
        public string tell_user { get; set; }
    }
}