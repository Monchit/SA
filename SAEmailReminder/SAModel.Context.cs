﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAEmailReminder
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SpecialAcceptanceEntities : DbContext
    {
        public SpecialAcceptanceEntities()
            : base("name=SpecialAcceptanceEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<TD_Main> TD_Main { get; set; }
        public DbSet<TD_Transaction> TD_Transaction { get; set; }
        public DbSet<TM_Level> TM_Level { get; set; }
        public DbSet<TM_Status> TM_Status { get; set; }
        public DbSet<TM_SysUser> TM_SysUser { get; set; }
    }
}
