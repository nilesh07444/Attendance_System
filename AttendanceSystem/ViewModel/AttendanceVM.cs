﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class AttendanceVM
    {
        public long AttendanceId { get; set; }
        public long CompanyId { get; set; }
        public long UserId { get; set; }
        [Display(Name = "Employee Name")]
        public string Name { get; set; }
        [Display(Name = "Attendance Date")]
        public DateTime AttendanceDate { get; set; }
        [Display(Name = "Day Type")]
        public double DayType { get; set; }
        public string DayTypeText { get; set; }
        [Display(Name = "Extra Hours")]
        public decimal ExtraHours { get; set; }
        [Display(Name = "Today Work detail")]
        public string TodayWorkDetail { get; set; }
        [Display(Name = "Tomorrow Work Detail")]
        public string TomorrowWorkDetail { get; set; }
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        [Display(Name = "In Location")]
        public string LocationFrom { get; set; }
        [Display(Name = "Out Location")]
        public string OutLocationFrom { get; set; }
        [Display(Name = "Status")]
        public int Status { get; set; }
        public string StatusText { get; set; }
        [Display(Name = "Reject Reason")]
        public string RejectReason { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "In Date Time")]
        public DateTime InDateTime { get; set; }
        [Display(Name = "Out Date Time")]
        public DateTime? OutDateTime { get; set; }
        public decimal? ExtraPerHourPrice { get; set; }
        public int EmploymentCategory { get; set; }
        public string EmploymentCategoryText { get; set; }
        public decimal? InLatitude { get; set; }
        public decimal? InLongitude { get; set; }
        public decimal? OutLatitude { get; set; }
        public decimal? OutLongitude { get; set; }
        [Display(Name = "No Of Hours Worked")]
        public decimal NoOfHoursWorked { get; set; }
        [Display(Name = "No Of Unit Worked")]
        public int NoOfUnitWorked { get; set; }

        public string EmployeeCode { get; set; }
        public decimal PerCategoryPrice { get; set; }
    }

    public class AttendanceFilterVM
    {
        public AttendanceFilterVM()
        {            
            Year = CommonMethod.CurrentIndianDateTime().Year;
            AttendanceStatus = (int)Helper.AttendanceStatus.Pending;
        }
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public int Year { get; set; }
        public int? AttendanceStatus { get; set; }
        public int? UserId { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public List<SelectListItem> CalenderMonth { get; set; }
        public List<AttendanceVM> AttendanceList { get; set; }
    }

    public class InTimeRequestVM
    {
        public string InLocationFrom { get; set; }
        public decimal InLatitude { get; set; }
        public decimal InLongitude { get; set; }
    }

    public class OutTimeRequestVM
    {
        public string OutLocationFrom { get; set; }
        public decimal OutLatitude { get; set; }
        public decimal OutLongitude { get; set; }
        public decimal ExtraHours { get; set; }
        public string TodayWorkDetail { get; set; }
        public string TomorrowWorkDetail { get; set; }
        public string Remarks { get; set; }
        public double DayType { get; set; }
        public decimal NoOfHoursWorked { get; set; }
        public int NoOfUnitWorked { get; set; }
    }

    public class WorkerAttendanceRequestVM
    {
        public int AttendanceType { get; set; }
        public long EmployeeId { get; set; }
        public decimal ExtraHours { get; set; }
        public decimal NoOfHoursWorked { get; set; }
        public int NoOfUnitWorked { get; set; }
        public long SiteId { get; set; }
        public decimal? TodaySalary { get; set; }

        public string LocationFrom { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
    public class WorkerAttendanceFilterVM
    {
        public WorkerAttendanceFilterVM()
        {
            AttendanceDate = CommonMethod.CurrentIndianDateTime().Date;
        }
        public DateTime AttendanceDate { get; set; }
        public int SiteId { get; set; }
        public long? EmployeeId { get; set; }

        public List<WorkerAttendanceVM> AttendanceList { get; set; }
        public List<SelectListItem> SiteList { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
    }

    public class WorkerAttendanceVM
    {
        public long AttendanceId { get; set; }
        public long CompanyId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string Name { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string ProfilePicture { get; set; }
        public int EmploymentCategory { get; set; }
        public string EmploymentCategoryText { get; set; }
        public bool IsMorning { get; set; }
        public string IsMorningText { get; set; }
        public bool IsAfternoon { get; set; }
        public string IsAfternoonText { get; set; }
        public bool IsEvening { get; set; }
        public string IsEveningText { get; set; }
    }

    public class WorkerAttendanceReportFilterVM
    {
        public WorkerAttendanceReportFilterVM()
        {
            StartDate = CommonMethod.CurrentIndianDateTime(); 
            EndDate = CommonMethod.CurrentIndianDateTime();
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? SiteId { get; set; }
        public long? EmployeeId { get; set; }
    }

    public class AddWorkerAttendanceVM
    {
        public long AttendanceId { get; set; }
        [Display(Name = "Attendance Date")]
        public DateTime AttendanceDate { get; set; }
        public int AttendanceType { get; set; }
        public long EmployeeId { get; set; }
        public long EmploymentCategoryId { get; set; }
        [Display(Name = "Employment Category")]
        public string EmploymentCategoryText { get; set; }
        [Display(Name = "Worker Name")]
        public string EmployeeName { get; set; }
        [Display(Name = "Worker Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Extra Hours")]
        public decimal ExtraHours { get; set; }
        [Display(Name = "No of Hours WOrked")]
        public decimal NoOfHoursWorked { get; set; }
        [Display(Name = "No of Unit WOrked")]
        public int NoOfUnitWorked { get; set; }
        public long SiteId { get; set; }
        [Display(Name = "Site")]
        public string SiteName { get; set; }
        public decimal? TodaySalary { get; set; }
        public string LocationFrom { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        [Display(Name = "Pending Salary")]
        public decimal PendingSalary { get; set; }
        public List<SelectListItem> WorkerAttendanceTypeList { get; set; }
        public decimal PerCategoryPrice { get; set; }
        public decimal? ExtraPerHourPrice { get; set; }
    }
}