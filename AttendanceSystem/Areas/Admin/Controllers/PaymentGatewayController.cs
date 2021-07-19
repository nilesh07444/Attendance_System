using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class PaymentGatewayController : Controller
    {
        AttendanceSystemEntities _db;
        public string packageDirectoryPath = "";
        public PaymentGatewayController()
        {
            _db = new AttendanceSystemEntities(); 
        }

        public ActionResult Index()
        {
            ViewBag.StripePublishKey = ConfigurationManager.AppSettings["stripePublishableKey"];


            StripeConfiguration.SetApiKey("pk_test_51IvP71SJ3WgQ7vkp7BwTganyt7h1m6xkNK5X04iY3ltxRAGSKxx8o0qzdXaqFg8iIX1NVM8sm71XUupmEhoHDK2P00YYj8L52C");
            StripeConfiguration.ApiKey = "sk_test_51IvP71SJ3WgQ7vkp6b6Za5tytc5urgyoBDvPah5czjw2zqnUzXQWoHwqwtq0CbdO2kSMxHFjhOHeDLuB7s67qwed00A7xWEN3S";

            string stripeEmail = "prajapati.nileshbhai@gmail.com";
            string stripeToken = "tok_1JCTfsSJ3WgQ7vkpphD7hdIS";

            var myCharge = new Stripe.ChargeCreateOptions();
            // always set these properties
            myCharge.Amount = 500;
            myCharge.Currency = "USD";
            myCharge.ReceiptEmail = stripeEmail;
            myCharge.Description = "Sample Charge";
            myCharge.Source = stripeToken;
            myCharge.Capture = true;
            var chargeService = new Stripe.ChargeService();
            Charge stripeCharge = chargeService.Create(myCharge);

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
         
        public string downloadTestPDF()
        {

            string Result = "";
            try
            {
                DateTime start_date = DateTime.UtcNow;
                DateTime end_date = DateTime.UtcNow;

                List<PackageVM> lstPackage = (from pck in _db.tbl_Package
                                              where !pck.IsDeleted
                                              select new PackageVM
                                              {
                                                  PackageId = pck.PackageId
                                              }).OrderByDescending(x => x.PackageId).ToList();

                decimal? TotalExpenseAmount = 1000;
                string strTotalExpenseAmount = CommonMethod.GetFormatterAmount(Convert.ToDecimal(TotalExpenseAmount));

                string[] strColumns = new string[5] { "Date", "Amount", "Type", "Site Name", "Description" };
                if (lstPackage != null && lstPackage.Count() > 0)
                {

                    List<DateTime> lstDateTemp = new List<DateTime>();
                    StringBuilder strHTML = new StringBuilder();
                    strHTML.Append("<!DOCTYPE html>");
                    strHTML.Append("<style>");
                    strHTML.Append("@page {@bottom-center {content: \"Page \" counter(page) \" of \" counter(pages);}}");
                    strHTML.Append("</style>");

                    strHTML.Append("<table cellspacing='0' border='1' cellpadding='5' style='width:100%; repeat-header:yes;repeat-footer:yes;border-collapse: collapse;border: 1px solid #000000;font-size: 12pt;page-break-inside:auto;'>");
                    strHTML.Append("<thead style=\"display:table-header-group;\">");
                    string Title = "Package List";
                    strHTML.Append("<tr>");
                    strHTML.Append("<th colspan=\"" + strColumns.Length + "\" style=\"border: 1px solid #000000\">");
                    strHTML.Append(Title);
                    strHTML.Append("</th>");
                    strHTML.Append("</tr>");
                    strHTML.Append("<tr><th colspan=\"" + strColumns.Length + "\" style=\"border: 1px solid #000000\">From " + start_date.ToString("dd/MM/yyyy") + " To " + end_date.ToString("dd/MM/yyyy") + " </th></tr>");
                    strHTML.Append("<tr>");
                    for (int idx = 0; idx < strColumns.Length; idx++)
                    {
                        strHTML.Append("<th style=\"border: 1px solid #000000\">");
                        strHTML.Append(strColumns[idx]);
                        strHTML.Append("</th>");
                    }
                    strHTML.Append("</tr>");
                    strHTML.Append("</thead>");
                    strHTML.Append("<tbody>");
                    foreach (var obj in lstPackage)
                    {

                        if (obj != null)
                        {

                            strHTML.Append("<tr style='page-break-inside:avoid; page-break-after:auto;'>");
                            for (int Col = 0; Col < strColumns.Length; Col++)
                            {
                                string strcolval = "";
                                switch (strColumns[Col])
                                {

                                    case "Date":
                                        {
                                            strcolval = DateTime.UtcNow.ToString("dd/MM/yyyy");
                                            break;
                                        }
                                    case "Amount":
                                        {
                                            strcolval = "500";
                                            break;
                                        }
                                    case "Type":
                                        {
                                            strcolval = "500";
                                            break;
                                        }
                                    case "Site Name":
                                        {
                                            strcolval = "Helios";
                                            break;
                                        }
                                    case "Description":
                                        {
                                            strcolval = "Gotri Road, Vadodara";
                                            break;
                                        }
                                    default:
                                        {
                                            break;
                                        }

                                }
                                strHTML.Append("<td style=\"width: auto; border: 1px solid #000000\">");
                                strHTML.Append(strcolval);
                                strHTML.Append("</td>");
                            }
                            strHTML.Append("</tr>");
                        }
                    }

                    // Total
                    strHTML.Append("<tr>");
                    strHTML.Append("<th style='text-align:right; border: 1px solid #000000;'>Total</th>");
                    strHTML.Append("<th style='border: 1px solid #000000;'> " + strTotalExpenseAmount + " </th>");
                    strHTML.Append("<th colspan='3' style='border: 1px solid #000000;'></th>");
                    strHTML.Append("</tr>");

                    strHTML.Append("</tbody>");
                    strHTML.Append("</table>");

                    StringReader sr = new StringReader(strHTML.ToString());

                    var myString = strHTML.ToString();
                    var myByteArray = System.Text.Encoding.UTF8.GetBytes(myString);
                    var ms = new MemoryStream(myByteArray);

                    Document pdfDoc = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    writer.PageEvent = new PDFGeneratePageEventHelper();
                    pdfDoc.Open();

                    XMLWorkerHelper objHelp = XMLWorkerHelper.GetInstance();
                    objHelp.ParseXHtml(writer, pdfDoc, ms, null, Encoding.UTF8, new UnicodeFontFactory());

                    pdfDoc.Close();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "download;filename=Test List" + ".pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Write(pdfDoc);
                    Response.End();
                }

                return Result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
            }

        }


    }
}