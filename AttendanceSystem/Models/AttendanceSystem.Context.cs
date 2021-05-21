﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AttendanceSystem.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class AttendanceSystemEntities : DbContext
    {
        public AttendanceSystemEntities()
            : base("name=AttendanceSystemEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<mst_AdminRole> mst_AdminRole { get; set; }
        public virtual DbSet<mst_CompanyType> mst_CompanyType { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<tbl_AdminUser> tbl_AdminUser { get; set; }
        public virtual DbSet<tbl_Company> tbl_Company { get; set; }
        public virtual DbSet<tbl_CompanyFollowup> tbl_CompanyFollowup { get; set; }
        public virtual DbSet<tbl_CompanyRenewPayment> tbl_CompanyRenewPayment { get; set; }
        public virtual DbSet<tbl_CompanyRequest> tbl_CompanyRequest { get; set; }
        public virtual DbSet<tbl_CompanySMSPackRenew> tbl_CompanySMSPackRenew { get; set; }
        public virtual DbSet<tbl_DynamicContent> tbl_DynamicContent { get; set; }
        public virtual DbSet<tbl_EmployeeBuyTransaction> tbl_EmployeeBuyTransaction { get; set; }
        public virtual DbSet<tbl_EmployeeFingerprint> tbl_EmployeeFingerprint { get; set; }
        public virtual DbSet<tbl_EmployeeRating> tbl_EmployeeRating { get; set; }
        public virtual DbSet<tbl_Feedback> tbl_Feedback { get; set; }
        public virtual DbSet<tbl_Holiday> tbl_Holiday { get; set; }
        public virtual DbSet<tbl_HomeImage> tbl_HomeImage { get; set; }
        public virtual DbSet<tbl_Leave> tbl_Leave { get; set; }
        public virtual DbSet<tbl_Material> tbl_Material { get; set; }
        public virtual DbSet<tbl_MaterialCategory> tbl_MaterialCategory { get; set; }
        public virtual DbSet<tbl_Package> tbl_Package { get; set; }
        public virtual DbSet<tbl_Setting> tbl_Setting { get; set; }
        public virtual DbSet<tbl_Site> tbl_Site { get; set; }
        public virtual DbSet<tbl_SMSPackage> tbl_SMSPackage { get; set; }
        public virtual DbSet<tbl_Sponsor> tbl_Sponsor { get; set; }
        public virtual DbSet<tbl_LoginHistory> tbl_LoginHistory { get; set; }
        public virtual DbSet<tbl_EmployeePayment> tbl_EmployeePayment { get; set; }
        public virtual DbSet<tbl_Employee> tbl_Employee { get; set; }
        public virtual DbSet<tbl_SMSLog> tbl_SMSLog { get; set; }
        public virtual DbSet<tbl_AssignWorker> tbl_AssignWorker { get; set; }
        public virtual DbSet<tbl_ContactForm> tbl_ContactForm { get; set; }
        public virtual DbSet<tbl_WorkerAttendance> tbl_WorkerAttendance { get; set; }
        public virtual DbSet<tbl_Attendance> tbl_Attendance { get; set; }
        public virtual DbSet<tbl_Testimonial> tbl_Testimonial { get; set; }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    }
}
