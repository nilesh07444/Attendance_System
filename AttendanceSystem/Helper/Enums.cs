using System.ComponentModel;

namespace AttendanceSystem.Helper
{

    public enum AdminRoles
    {
        SuperAdmin = 1,
        CompanyAdmin = 2,
        Employee = 3,
        Supervisor = 4,
        Checker = 5,
        Payer = 6,
        Worker = 7
    }

    public enum AttendanceStatus
    {
        Pending = 1,
        Accept = 2,
        Reject = 3,
        Login = 4
    }

    public enum LeaveStatus
    {
        Pending = 1,
        Accept = 2,
        Reject = 3,
    }

    public enum EmployeePaymentType
    {
        Salary = 1,
        Extra = 2
    }
    public enum Status
    {
        Failure = 0,
        Success = 1
    }

    public enum HomeImageFor
    {
        Website = 1,
        MobileApp = 2
    }
    public enum CompanyRequestStatus
    {
        Pending = 1,
        Accept = 2,
        Reject = 3
    }

    public enum CompanyType
    {
        Banking_OfficeCompany = 1,
        ConstructionCompany = 2
    }

    public enum DynamicContents
    {
        FAQ = 1,
        PrivacyPolicy = 2,
        TermsCondition = 3,
        HowToUse = 4
    }

    public enum EmploymentCategory
    {
        [Description("Daily Based")]
        DailyBased = 1,
        [Description("Hourly Based")]
        HourlyBased = 2,
        [Description("Monthly Based")]
        MonthlyBased = 3,
        [Description("Unit Based")]
        UnitBased = 4
    }

    public enum UserStatus
    {
        Active = 1,
        InActive = 0
    }

    public enum FeedbackType
    {
        Query = 1,
        Suggestion = 2
    }

    public enum FeedbackStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Resolved")]
        Resolved = 2,
        [Description("Not Resolved")]
        NotResolved = 3
    }

    public enum FollowupStatus
    {
        [Description("Open")]
        Open = 1,
        [Description("Interested")]
        Interested = 2,
        [Description("Not Interested")]
        NotInterested = 3,
        [Description("Close")]
        Close = 4
    }

    public enum MaterialStatus
    {
        Inward = 1,
        Outward = 2, 
        Open = 3
    }

    public enum Months
    {
        StartMonth = 1,
        EndMonth = 12
    }

    public enum WorkerAttendanceType
    {
        Morning = 1,
        Afternoon = 2,
        Evening = 3
    }

    public enum CalenderMonths
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public enum SMSType
    {
        SuperAdminLoginOTP = 1,
        CompanyAdminLoginOTP = 2,
        EmployeeLoginOTP = 3,
        ForgetPasswordOTP = 4,
        ChangePasswordOTP = 5,
        CompanyProfileEditOTP = 6,
        CompanyRequest = 7,
        CompanyRequestAccepted = 8,
        CompanyRequestRejected = 9,
        PackageBuy = 10,
        EmployeeCreate = 11,
        AttendanceRejected = 12,
        LeaveApproved = 13,
        LeaveRejected = 14,
        PaymentOTP = 15,
        EmployeeProfileEditOTP = 16,
        CompanyRequestOTP = 17,
        EmployeeCreateOTP = 18
    }

    public enum DayType
    {
        [Description("Full Day")]
        FullDay = 1,
        [Description("Half Day")]
        HalfDay = 2
    }
    public enum PaymentGivenBy
    {
        CompanyAdmin = 0,
        SuperAdmin = -1
    }
    public enum SalesReportType
    {
        Account = 1,
        SMS = 2,
        Employee = 3
    }
    public enum EmployeeOfficeLocationType
    {
        [Description("Anywhere")]
        Anywhere = 1,
        [Description("All Offices")]
        AllOffices = 2,
        [Description("Selected Office")]
        SelectedOffice = 3
    }

    public enum CompanyConversionType
    {
        MonthBased = 1,
        DayBased = 2
    }

}