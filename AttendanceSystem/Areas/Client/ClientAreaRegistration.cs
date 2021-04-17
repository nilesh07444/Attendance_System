using System.Web.Mvc;

namespace AttendanceSystem.Areas.Client
{
    public class ClientAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Client";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Client_default",
                "client/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            // Home Page
            context.MapRoute(
                "Client_HomePage",
                "home",
                new { controller = "Main", action = "Index", id = UrlParameter.Optional }
            );

            // Contact Page
            context.MapRoute(
                "Client_Contact",
                "contact",
                new { controller = "Contact", action = "Index", id = UrlParameter.Optional }
            );

            // About Page
            context.MapRoute(
                "Client_About",
                "about",
                new { controller = "About", action = "Index", id = UrlParameter.Optional }
            );

            // FAQ Page
            context.MapRoute(
                "Client_FAQ",
                "faq",
                new { controller = "FAQ", action = "Index", id = UrlParameter.Optional }
            );

            // Company Request Page
            context.MapRoute(
                "Client_CompanyRequest",
                "companyrequest",
                new { controller = "CompanyRequest", action = "Index", id = UrlParameter.Optional }
            );

            // Privacy Policy Page
            context.MapRoute(
                "Client_PrivacyPolicy",
                "privacypolicy",
                new { controller = "PrivacyPolicy", action = "Index", id = UrlParameter.Optional }
            );

            // Terms Condition Page
            context.MapRoute(
                "Client_TermsCondition",
                "termscondition",
                new { controller = "TermsCondition", action = "Index", id = UrlParameter.Optional }
            );

            // Webview Pages
            context.MapRoute(
                "Client_WebView_TermsCondition",
                "webview/termscondition",
                new { controller = "WebView", action = "TermsCondition", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_WebView_Privacy",
                "webview/privacypolicy",
                new { controller = "WebView", action = "PrivacyPolicy", id = UrlParameter.Optional }
            );

        }
    }
}