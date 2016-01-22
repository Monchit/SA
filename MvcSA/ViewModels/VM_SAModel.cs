using MvcSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSA.ViewModels
{
    public class VM_SAModel
    {
        public IEnumerable<TD_Disposition> Disposition { get; set; }
        public IEnumerable<TD_File> File { get; set; }
        public IEnumerable<TD_Main> Main { get; set; }
        public IEnumerable<TD_CarPar> PcrCar { get; set; }
        public IEnumerable<TD_Tell> Tell { get; set; }
        public IEnumerable<TD_Transaction> Transaction { get; set; }

        public IEnumerable<TM_Action> M_Action { get; set; }
        public IEnumerable<TM_Admin> M_Admin { get; set; }
        public IEnumerable<TM_Disposition> M_Disposition { get; set; }
        public IEnumerable<TM_Level> M_Level { get; set; }
        public IEnumerable<TM_Problem> M_Problem { get; set; }
        public IEnumerable<TM_Status> M_Status { get; set; }
        public IEnumerable<TM_SysPlant> M_SysPlant { get; set; }
    }
}