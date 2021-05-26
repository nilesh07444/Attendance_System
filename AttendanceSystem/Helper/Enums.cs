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
        TermsCondition = 3
    }

    public enum EmploymentCategory
    {
        DailyBased = 1,
        HourlyBased = 2,
        MonthlyBased = 3,
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
        Pending = 1,
        Resolved = 2,
        NotResolved = 3
    }

    public enum FollowupStatus
    {
        Open = 1,
        Close = 2,
        NotInterested = 3
    }

    public enum MateriaStatus
    {
        In = 1,
        Out = 2
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
        SuperAdminLoginOtp = 1,
        CompanyAdminLoginOtp = 2,
        EmployeeLoginOtp = 3,
    }

    public enum DayType
    {
        [Description("Full Day")]
        FullDay = 1,
        [Description("Half Day")]
        HalfDay = 2

    }
}

