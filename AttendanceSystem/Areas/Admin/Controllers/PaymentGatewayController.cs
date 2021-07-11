using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class PaymentGatewayController : Controller
    {
        // GET: Admin/PaymentGateway
        public ActionResult Index()
        {
            ViewBag.StripePublishKey = ConfigurationManager.AppSettings["stripePublishableKey"];
            return View();
        }

        //[HttpPost]
        //public ActionResult Charge(string stripeToken, string stripeEmail)
        //{
        //    Stripe.StripeConfiguration.SetApiKey("pk_test_51J0dCDSHPBrMzwLwUJro7gGUprYVNdVKTz51icD7wHLCcML9E2PwpIBIcdGfjjqsYx4BYG7Oz5bwIg2ZfRvr37WZ00cP2ivuR7");
        //    Stripe.StripeConfiguration.ApiKey = "sk_test_51J0dCDSHPBrMzwLwvuMSiHrey5q2y9e2rwWWnHJE0XxgOtA2z5koG8WvR8a6llsbxBhtMX85cGcw5ykJhCJL3vGF00uSgi0caW";

        //    var myCharge = new Stripe.ChargeCreateOptions();
        //    // always set these properties
        //    myCharge.Amount = 500;
        //    myCharge.Currency = "USD";
        //    myCharge.ReceiptEmail = stripeEmail;
        //    myCharge.Description = "Sample Charge";
        //    myCharge.Source = stripeToken;
        //    myCharge.Capture = true;
        //    var chargeService = new Stripe.ChargeService();
        //    Charge stripeCharge = chargeService.Create(myCharge);
        //    return View();
        //}

        [HttpPost]
        public ActionResult Charge(string stripeEmail, string stripeToken)
        {
            Stripe.CustomerCreateOptions customer = new Stripe.CustomerCreateOptions();

            var custService = new Stripe.CustomerService();

            Stripe.Customer stpCustomer = custService.Create(customer);

            return View();
        }
    }
}