using System;
using System.Web;

namespace AttendanceSystem.Helper
{
    public class clsAdminSession
    {

        public static long UserID
        {
            get
            {
                return HttpContext.Current.Session["UserID"] != null ? Int32.Parse(Convert.ToString(HttpContext.Current.Session["UserID"])) : 0;
            }
            set
            {
                HttpContext.Current.Session["UserID"] = value;
            }
        }
        public static int RoleID
        {
            get
            {
                return HttpContext.Current.Session["RoleID"] != null ? Int32.Parse(Convert.ToString(HttpContext.Current.Session["RoleID"])) : 0;
            }
            set
            {
                HttpContext.Current.Session["RoleID"] = value;
            }
        }
        public static String SessionID
        {
            get
            {
                return HttpContext.Current.Session["SessionID"] != null ? Convert.ToString(HttpContext.Current.Session["SessionID"]) : String.Empty;
            }
            set
            {
                HttpContext.Current.Session["SessionID"] = value;
            }
        }
        public static String RoleName
        {
            get
            {
                return HttpContext.Current.Session["RoleName"] != null ? Convert.ToString(HttpContext.Current.Session["RoleName"]) : String.Empty;
            }
            set
            {
                HttpContext.Current.Session["RoleName"] = value;
            }
        }

        public static String FirmName
        {
            get
            {
                return HttpContext.Current.Session["FirmName"] != null ? Convert.ToString(HttpContext.Current.Session["FirmName"]) : String.Empty;
            }
            set
            {
                HttpContext.Current.Session["FirmName"] = value;
            }
        }
        public static string ImagePath
        {
            get
            {
                return HttpContext.Current.Session["ImagePath"] != null ? Convert.ToString(HttpContext.Current.Session["ImagePath"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["ImagePath"] = value;
            }
        }
        public static string UserName
        {
            get
            {
                return HttpContext.Current.Session["UserName"] != null ? Convert.ToString(HttpContext.Current.Session["UserName"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["UserName"] = value;
            }
        }

        public static string FullName
        {
            get
            {
                return HttpContext.Current.Session["FullName"] != null ? Convert.ToString(HttpContext.Current.Session["FullName"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["FullName"] = value;
            }
        }

        public static string MobileNumber
        {
            get
            {
                return HttpContext.Current.Session["AdminMobileNumber"] != null ? Convert.ToString(HttpContext.Current.Session["AdminMobileNumber"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["AdminMobileNumber"] = value;
            }
        }

        public static string UserPermission
        {
            get
            {
                return HttpContext.Current.Session["UserPermission"] != null ? Convert.ToString(HttpContext.Current.Session["UserPermission"]) : string.Empty;
            }
            set
            {
                HttpContext.Current.Session["UserPermission"] = value;
            }
        }

        public static long CompanyId
        {
            get
            {
                return HttpContext.Current.Session["CompanyId"] != null ? Int32.Parse(Convert.ToString(HttpContext.Current.Session["CompanyId"])) : 0;
            }
            set
            {
                HttpContext.Current.Session["CompanyId"] = value;
            }
        }

        public static long CompanyTypeId
        {
            get
            {
                return HttpContext.Current.Session["CompanyTypeId"] != null ? Int32.Parse(Convert.ToString(HttpContext.Current.Session["CompanyTypeId"])) : 0;
            }
            set
            {
                HttpContext.Current.Session["CompanyTypeId"] = value;
            }
        }

        public static bool IsTrialMode
        {
            get
            {
                return HttpContext.Current.Session["IsTrialMode"] != null ? Convert.ToBoolean(Convert.ToString(HttpContext.Current.Session["IsTrialMode"])) : false;
            }
            set
            {
                HttpContext.Current.Session["IsTrialMode"] = value;
            }
        }
    }
}
