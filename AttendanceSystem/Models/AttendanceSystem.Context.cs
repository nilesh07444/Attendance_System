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
    
        public virtual DbSet<tbl_AdminUser> tbl_AdminUser { get; set; }
        public virtual DbSet<mst_AdminRole> mst_AdminRole { get; set; }
        public virtual DbSet<mst_CompanyType> mst_CompanyType { get; set; }
        public virtual DbSet<tbl_Site> tbl_Site { get; set; }
        public virtual DbSet<tbl_DynamicContent> tbl_DynamicContent { get; set; }
        public virtual DbSet<tbl_Package> tbl_Package { get; set; }
        public virtual DbSet<tbl_Setting> tbl_Setting { get; set; }
        public virtual DbSet<tbl_CompanyRenewPayment> tbl_CompanyRenewPayment { get; set; }
        public virtual DbSet<tbl_CompanyRequest> tbl_CompanyRequest { get; set; }
        public virtual DbSet<tbl_Company> tbl_Company { get; set; }
        public virtual DbSet<tbl_Holiday> tbl_Holiday { get; set; }
        public virtual DbSet<tbl_LoginHistory> tbl_LoginHistory { get; set; }
        public virtual DbSet<tbl_Employee> tbl_Employee { get; set; }
        public virtual DbSet<tbl_EmployeePayment> tbl_EmployeePayment { get; set; }
        public virtual DbSet<tbl_CompanyFollowup> tbl_CompanyFollowup { get; set; }
        public virtual DbSet<tbl_CompanySMSPackRenew> tbl_CompanySMSPackRenew { get; set; }
        public virtual DbSet<tbl_EmployeeRating> tbl_EmployeeRating { get; set; }
        public virtual DbSet<tbl_Feedback> tbl_Feedback { get; set; }
        public virtual DbSet<tbl_Leave> tbl_Leave { get; set; }
        public virtual DbSet<tbl_MaterialCategory> tbl_MaterialCategory { get; set; }
        public virtual DbSet<tbl_Material> tbl_Material { get; set; }
        public virtual DbSet<tbl_HomeImage> tbl_HomeImage { get; set; }
        public virtual DbSet<tbl_SMSPackage> tbl_SMSPackage { get; set; }
        public virtual DbSet<tbl_Attendance> tbl_Attendance { get; set; }
    }
}
