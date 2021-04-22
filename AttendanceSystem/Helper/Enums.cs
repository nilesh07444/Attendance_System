﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        Reject = 3
    }

    public enum LeaveStatus
    {
        Pending = 1,
        Accept = 2,
        Reject = 3,
        Cancelled = 4
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

    public enum DynamicContents
    {
        FAQ = 1,
        PrivacyPolicy = 2,
        TermsCondition = 3 
    }

}