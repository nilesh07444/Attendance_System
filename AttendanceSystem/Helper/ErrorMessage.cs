namespace AttendanceSystem.Helper
{
    public static class ErrorMessage
    {
        // Admin Portal Messages
        public static string GstNumberExists = "GST Number is already exist";
        public static string YouAreNotAuthorized = "You are not authorized. Please contact your administrator.";
        public static string InvalidCredentials = "Please enter valid UserName or Password.";
        public static string InvalidMobileNo = "Invalid mobile no provided.";
        public static string InvalidPassword = "Invalid current password.";
        public static string InvalidUserName = "Invalid User Name.";
        public static string UserNamePasswordRequired = "User Name and Password required.";

        // Folder Directory Path
        public static string UserDirectoryPath = "/Images/UserMedia/";

        // Default Images
        public static string DefaultImagePath = "/Images/default_image.png";
        public static string DefaultUserImagePath = "/Images/default_user_image.png";
        public static string HomeDirectoryPath = "/Images/HomeImage/";
        public static string PackageDirectoryPath = "/Images/PackageImage/";

        public static string TokenExpired = "Token expired";
        public static string InvalidToken = "Invalid token";
        public static string AuthorizationTokenMissing = "Authorization token missing";
        public static string InternalServerError = "Internal server error";
        public static string SelectOnlyImage = "Select only image file.";
        public static string ImageRequired = "Please select image file.";
    }
}