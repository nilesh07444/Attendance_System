namespace AttendanceSystem.Helper
{
    public static class ErrorMessage
    {
        // Admin Portal Messages
        public static string GstNumberExists = "GST Number is already exist";
        public static string UserNameOrPasswordInvalid = "Username Or Password Invalid.";
        public static string InvalidCredentials = "Please enter valid UserName or Password.";
        public static string InvalidMobileNo = "Invalid mobile no provided.";
        public static string InvalidPassword = "Invalid current password.";
        public static string InvalidUserName = "Invalid User Name.";
        public static string UserNamePasswordRequired = "User Name and Password required.";
        public static string CurrentAndNewBothPasswordRequired = "Current and new both password required.";
        public static string NewPasswordAndConfirmPasswordMustBeSame = "New password and confirm password must be same.";

        // Folder Directory Path
        public static string UserDirectoryPath = "/Images/UserMedia/";

        // Default Images
        public static string DefaultImagePath = "/Images/default_image.png";
        public static string DefaultUserImagePath = "/Images/default_user_image.png";
        public static string HomeDirectoryPath = "/Images/HomeImage/";
        public static string PackageDirectoryPath = "/Images/PackageImage/";
        public static string SMSPackageDirectoryPath = "/Images/SMSPackageImage/";
        public static string PancardDirectoryPath = "/Images/Pancard/";
        public static string AdharcardDirectoryPath = "/Images/Adharcard/";
        public static string GSTDirectoryPath = "/Images/GST/";
        public static string CompanyDirectoryPath = "/Images/CompanyPhoto/";
        public static string CancellationChequeDirectoryPath = "/Images/CancellationCheque/";
        public static string EmployeeDirectoryPath = "/Images/Employee/";
        public static string CompanyGSTDirectoryPath = "/Images/CompanyGST/";
        public static string CompanyPanCardDirectoryPath = "/Images/CompanyPanCard/";
        public static string CompanyLogoDirectoryPath = "/Images/CompanyLogo/";
        public static string CompanyRegisterProofDirectoryPath = "/Images/CompanyRegisterProof/";
        public static string OurClientDirectoryPath = "/Images/OurClient/";
        public static string ServiceDirectoryPath = "/Images/ServiceImage/";
        public static string ProfileDirectoryPath = "/Images/Profile/";

        public static string TokenExpired = "Token expired";
        public static string InvalidToken = "Invalid token";
        public static string AuthorizationTokenMissing = "Authorization token missing";
        public static string InternalServerError = "Internal server error";
        public static string SelectOnlyImage = "Select only image file.";
        public static string ImageRequired = "Please select image file.";
        public static string RunningStatusApprove = "Approve";
        public static string RunningStatusReject = "Reject";
        public static string RunningStatusPending = "Pending";
        public static string HolidayOnSameDateAlreadyExist = "Holiday on same date already exist, Please use another date.";
        public static string HolidayStartAndEndDateShouldBeforSameMonth = "Holiday start and end date should be for same month.";

        public static string LeaveDateRequired = "Leave date required";
        public static string StartDateCanNotBeGreaterThanEndDate = "Start date can not be greater than end date";
        public static string LeaveReasonRequired = "Leave reason required";
        public static string LeaveStartAndEndDateshouldBeForSameMonth = "Leave start and end date should be for same month";
        public static string LeaveOnSameDateAlreadyExist = "Leave on same date already exist";
        public static string LeaveDateCanBeFutureDateOnly = "Leave date can be future date only.";
        public static string LeaveIdIsNotValid = "Leave id is not valid.";
        public static string PendingLeaveCanBeEditOnly = "Only pending leave can be edit.";
        public static string PendingLeaveCanBeDeleteOnly = "Only pending leave can be Delete.";
        public static string LeaveNotFound = "Leave not found.";

        public static string MaterialCategoryIsNotValid = "Material category is not valid.";
        public static string SiteIsNotValid = "Site is not valid.";
        public static string MaterialQtyNotValid = "Material qty is not valid.";
        public static string MaterialInOutNotValid = "Material in/out is not valid.";

        public static string AttendanceDateRequired = "Attendance date required";
        public static string AttendanceDayTypeNotValid = "Attendance day type not valid";
        public static string TodayWorkDetailRequired = "Today work detail required";
        public static string TomorrowWorkDetailRequired = "Tomorrow work detail required";
        public static string InTimeIsNotValid = "In time is not valid";
        public static string OutTimeIsNotValid = "Out time is not valid";
        public static string AttendanceOnSameDateAlreadyExist = "Attendance on same date already exist";
        public static string AttendanceIdIsNotValid = "Attendance id is not valid.";
        public static string PendingAttendanceCanBeEditOnly = "Only pending attendance can be edit.";
        public static string PendingAttendanceCanBeDeleteOnly = "Only pending attendance can be Delete.";
        public static string AttendanceIsAcceptedCanNotDelete = "Attendance is accepted, can not Delete.";

        public static double FullDay = 1;
        public static double HalfDay = 0.5;
        public static string DefaultTime = "00:00:00";

        public static string EmployeeRatingAlreadyExist = "Employee rating already exist.";
        public static string MonthShouldBeFrom1To12 = "Month should be from 1 to 12.";
        public static string FutureYearNotAllowed = "Future year not allowed.";
        public static string MobileNoNotFoundForTheCompany = "Mobile no not found for the company.";
        public static string EmployeeIdIsNotValid = "Employee id is not valid.";
        public static string EmployeeDoesNotExist = "Employee does not exist.";
        public static string CompanyNameMinimum2CharacterRequired = "Company name minimum 2 character required.";

        public static string CompanyIsNotActive = "Company is not active.";
        public static string CompanyAccountIsExpired = "Company account is expired.";
        public static string CompanyTrailAccountIsExpired = "Company trail account is expired.";

        public static string SMSPackageIsExpired = "SMS package is expired.";
        public static string WorkerCanAssignForTodayOnly = "Workers can assign for today only.";
        public static string WorkerCanCloseForTodayOnly = "Workers can close for today only.";
        public static string SiteDoesNotExistForCurrentCompany = "Site does not exist for current company.";
        public static string WorkerAlreadyClosed = "Worker already closed.";
        public static string WorkerDidNotAssignedToThisSite = "Worker did not assigned to this site.";
        public static string InvalidWorkersSelectedToAssign = "Invalid workers selected to assign.";
        public static string WorkerFingerprintLimitReached = "Worker fingerprint limit reached";

        public static string AlreadyInForTheDay = "Already in for the day.";
        public static string AlreadyOutForTheDay = "Already out for the day.";
        public static string YouAreOnLeaveForTheday = "You are on leave the day.";
        public static string PleaseProvideDayType = "Please provide day type.";
        public static string PleaseProvideNoOfHoursWorked = "Please provide no of hours worked.";

        public static string PackageAlreadyAssigned = "PackageAlreadyAssigned";
        public static string Success = "success";
        public static string NotFound = "notfound";
        public static string Exception = "exception";
        public static string MaterialExistForThisCategory = "Material exist for this category.";

        public static string CheckerCanNotTakeEveningAttendance = "Checker can not take evening attendance.";
        public static string PayerCanNotTakeMorningOrAfterNoonAttendance = "Payer can not take morning or afternoon attendance.";
        public static string PayerOrCheckerOnlyCanTakeWorkerAttendance = "Payer or checker only can take worker attendance.";
        public static string InValidWorkerAttendanceType = "Invalid worker attendance type.";
        public static string CanNotTakeEveningAttendanceWorkerNotPresentOnAfterNoon = "Can not take evening attendance worker not present on afternoon.";
        public static string WorketNotAssignedToday = "Worker not assigned today.";
        public static string PleaseSelectValidMonth = "Please select valid month";

        public static string PleaseBuyAccountPackage = "Please buy account package.";
        public static string WorkerExistForThisWorkerType = "Worker exist for this worker type.";
        public static string PerDayPriceRequiredForDailyBasedWorker = "Per day price required for daily based worker.";
        public static string PerhourPriceRequiredForHourlyBasedWorker = "Per hour price required for hourly based worker.";
        public static string MonthlySalaryRequiredForMonthlyBasedWorker = "Monthly salary required for monthly based worker.";
        public static string ExtraPerHourPriceRequired = "Extra per hour price required.";
        public static string PerUnitPriceRequiredForUnitBasedWorker = "Per unit price required for unit based worker.";
        public static string EmploymentCategoryRequired = "Employment category required.";
        public static string WorketTypeRequired = "Worker type required.";

        public static string WorkerMorningAttendanceAlreadyDone = "Worker morning attendance already done.";
        public static string WorkerAfternoonAttendanceAlreadyDone = "Worker afternoon attendance already done.";
        public static string WorkerEveningAttendanceAlreadyDone = "Worker evening attendance already done.";

        public static string Credit = "Credit";
        public static string Debit = "Debit";
        public static string AutoCreditOnAttendanceAccept = "Auto credit on attendance accept";
        public static string AutoCreditOnEveningAttendance = "Auto credit on evening attendance";
    }
}