using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSA.ViewModels
{
    public class VM_AttachFiles
    {
        public int file_id { get; set; }
        public Nullable<int> id { get; set; }
        public string file_path { get; set; }
        public Nullable<System.DateTime> upload_dt { get; set; }
        public string upload_by { get; set; }
    }
}