using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMobileApp.Helper
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

        // Folder Directory Path
        public static string UserDirectoryPath = "/Images/UserMedia/";

        // Default Images
        public static string DefaultImagePath = "/Images/default_image.png";
        public static string DefaultUserImagePath = "/Images/default_user_image.png";

    }
}