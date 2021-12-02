using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace AttendanceSystem
{
    public static class CommonMethod
    {
        //public static string ConvertFromUTC(DateTime? utcDateTime)
        //{
        //    if (utcDateTime == null)
        //        return "";

        //    TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    DateTime dateTimeAsTimeZone = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(utcDateTime), nzTimeZone);
        //    string dt = dateTimeAsTimeZone.ToString("dd/MM/yyyy hh:mm tt");
        //    return dt;
        //}

        //public static string ConvertFromUTCNew(DateTime? utcDateTime)
        //{
        //    if (utcDateTime == null)
        //        return "";

        //    TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    DateTime dateTimeAsTimeZone = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(utcDateTime), nzTimeZone);
        //    string dt = dateTimeAsTimeZone.ToString("dd-MMM-yyyy hh:mm tt");
        //    return dt;
        //}

        //public static string ConvertFromUTCOnlyDate(DateTime? utcDateTime)
        //{
        //    if (utcDateTime == null)
        //        return "";

        //    TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    DateTime dateTimeAsTimeZone = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(utcDateTime), nzTimeZone);
        //    string dt = dateTimeAsTimeZone.ToString("dd/MM/yyyy");
        //    return dt;
        //}

        //public static DateTime ConvertFromUTCToIndianDateTime(DateTime utcDateTime)
        //{
        //    TimeZoneInfo indTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    DateTime dateTimeAsTimeZone = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, indTimeZone);
        //    return dateTimeAsTimeZone;
        //}
        public static DateTime CurrentIndianDateTime()
        {
            DateTime DT = DateTime.UtcNow;
            TimeZoneInfo indTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime dateTimeAsTimeZone = TimeZoneInfo.ConvertTimeFromUtc(DT, indTimeZone);
            return dateTimeAsTimeZone;
        }
        public static DateTime ConvertFromIndianTimeZoneToUTC(DateTime dateTime)
        {
            TimeZoneInfo indTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, indTimeZone);
            return utcDateTime;
        }
        public static DateTime ConvertToUTC(DateTime dateTime, string timeZone)
        {
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, nzTimeZone);
            return utcDateTime;
        }

        public static List<List<T>> Split<T>(this List<T> items, int sliceSize = 30)
        {
            List<List<T>> list = new List<List<T>>();
            for (int i = 0; i < items.Count; i += sliceSize)
                list.Add(items.GetRange(i, Math.Min(sliceSize, items.Count - i)));
            return list;
        }

        public static double GetRoundedValue(double amt)
        {
            double originalVal = amt;  // value before round off

            double roudedVal = Math.Round(originalVal, MidpointRounding.AwayFromZero); // rounded value

            double GetVal = 0;  // to know rounded value

            GetVal -= originalVal - roudedVal;

            GetVal = Math.Round(GetVal, 2);  // to again roundup value with just 2 decimal precision, if it 
            return GetVal;
        }

        public static double GetRoundValue(double amt)
        {
            double originalVal = amt;  // value before round off

            double roudedVal = Math.Round(originalVal, MidpointRounding.AwayFromZero); // rounded value          

            return roudedVal;
        }

        private static string ConvertWholeNumber(String Number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX    
                bool isDone = false;//test if already translated    
                double dblAmt = (Convert.ToDouble(Number));
                //if ((dblAmt > 0) && number.StartsWith("0"))    
                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric    
                    beginsZero = Number.StartsWith("0");

                    int numDigits = Number.Length;
                    int pos = 0;//store digit grouping    
                    String place = "";//digit grouping name:hundres,thousand,etc...    
                    switch (numDigits)
                    {
                        case 1://ones' range    

                            word = ones(Number);
                            isDone = true;
                            break;
                        case 2://tens' range    
                            word = tens(Number);
                            isDone = true;
                            break;
                        case 3://hundreds' range    
                            pos = (numDigits % 3) + 1;
                            place = " Hundred ";
                            break;
                        case 4://thousands' range    
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 7://millions' range    
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Million ";
                            break;
                        case 10://Billions's range    
                        case 11:
                        case 12:

                            pos = (numDigits % 10) + 1;
                            place = " Billion ";
                            break;
                        //add extra case options for anything above Billion...    
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)    
                        if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0")
                        {
                            try
                            {
                                word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));
                            }
                            catch { }
                        }
                        else
                        {
                            word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));
                        }

                        //check for trailing zeros    
                        //if (beginsZero) word = " and " + word.Trim();    
                    }
                    //ignore digit grouping names    
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch { }
            return word.Trim();
        }

        private static string ones(string Number)
        {
            int _Number = Convert.ToInt32(Number);
            string name = "";
            switch (_Number)
            {

                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }

        private static string tens(string Number)
        {
            int _Number = Convert.ToInt32(Number);
            string name = null;
            switch (_Number)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Fourty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (_Number > 0)
                    {
                        name = tens(Number.Substring(0, 1) + "0") + " " + ones(Number.Substring(1));
                    }
                    break;
            }
            return name;
        }

        public static string ConvertToWords(string numb)
        {
            string val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
            string endStr = "Only";
            try
            {
                int decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) > 0)
                    {
                        andStr = "and";// just to separate whole numbers from points/cents    
                        endStr = "Paisa " + endStr;//Cents    
                        pointStr = ConvertDecimals(points);
                    }
                }
                val = string.Format("{0} {1}{2} {3}", ConvertWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
            }
            catch { }
            return val;
        }

        private static string ConvertDecimals(string number)
        {
            string cd = "", digit = "", engOne = "";
            for (int i = 0; i < number.Length; i++)
            {
                digit = number[i].ToString();
                if (digit.Equals("0"))
                {
                    engOne = "Zero";
                }
                else
                {
                    engOne = ones(digit);
                }
                cd += " " + engOne;
            }
            return cd;
        }

        public static string GetRandomPassword(int length)
        {
            Random random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string randomPassword = new string(chars.Select(c => chars[random.Next(chars.Length)]).Take(length).ToArray());
            return randomPassword;
        }

        public static string GetSMSUrl()
        {
            string smsurl = WebConfigurationManager.AppSettings["SMSUrl"];
            return smsurl;
        }

        public static string Encrypt(string strToEncrypt, string strKey)
        {
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

                byte[] byteHash, byteBuff;
                string strTempKey = strKey;

                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

                byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
                return Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        public static string Decrypt(string strEncrypted, string strKey)
        {
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

                byte[] byteHash, byteBuff;
                string strTempKey = strKey;

                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

                byteBuff = Convert.FromBase64String(strEncrypted);
                string strDecrypted = ASCIIEncoding.ASCII.GetString(objDESCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                objDESCrypto = null;

                return strDecrypted;
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        public static void AppLog(string TextMessage)
        {
            try
            {
                var line = Environment.NewLine + Environment.NewLine;
                string filepath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();                //Text File Path

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);

                }
                filepath = filepath + "\\" + CommonMethod.CurrentIndianDateTime().ToString("dd-MM-yy") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine("-----------Exception Details on " + " " + CommonMethod.CurrentIndianDateTime().ToString() + "-----------------");
                    sw.WriteLine(TextMessage);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();

                }

            }
            catch (Exception e)
            {
                e.ToString();

            }
        }

        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public static int WeekDaysInMonth(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            List<DateTime> dates = new List<DateTime>();
            for (int i = 1; i <= days; i++)
            {
                dates.Add(new DateTime(year, month, i));
            }

            int weekDays = dates.Where(d => d.DayOfWeek > DayOfWeek.Sunday & d.DayOfWeek < DayOfWeek.Saturday).Count();
            return weekDays;
        }
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public static string getEmployeeCodeFormat(long companyId, string companyName, int lastEmpNumber)
        {
            string empCode = string.Empty;
            try
            {
                int newEmpNumber = lastEmpNumber + 1;
                string companyNameWithoutSpeChar = Regex.Replace(companyName, @"[^0-9a-zA-Z]+", "");
                string first2CharOfCompanyName = companyNameWithoutSpeChar.ToUpper().Substring(0, 2);
                empCode = first2CharOfCompanyName + "/EMP/" + companyId + "/" + newEmpNumber;
            }
            catch (Exception ex)
            {
            }
            return empCode;
        }

        public static string SendSMSWithoutLog(string msg, string mobileNo)
        {
            using (WebClient webClient = new WebClient())
            {
                msg = HttpUtility.UrlEncode(msg);
                string url = GetSMSUrl().Replace("--MOBILE--", mobileNo).Replace("--MSG--", msg);
                var json = webClient.DownloadString(url);
                return json;
            }
        }

        public static ResponseDataModel<string> SendSMS(string msg, string mobileNo, long companyId, long employeeId, string employeeCode, long loggedInUser, bool isTrialMode = false)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {
                if (!isTrialMode)
                {
                    DateTime currentDateTime = CommonMethod.CurrentIndianDateTime();

                    AttendanceSystemEntities _db = new AttendanceSystemEntities();
                    tbl_CompanySMSPackRenew activeSMSPackage = null;

                    tbl_Company companyObj = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                    tbl_CompanySMSPackRenew currentSMSPackage = _db.tbl_CompanySMSPackRenew.Where(x => x.CompanyId == companyId
                    && x.RenewDate <= currentDateTime
                    && x.PackageExpiryDate > currentDateTime
                    && (companyObj.CurrentSMSPackageId.HasValue ? x.CompanySMSPackRenewId == companyObj.CurrentSMSPackageId.Value : true)).FirstOrDefault();

                    //could not found any active package 
                    if (currentSMSPackage == null)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.SMSPackageIsExpired);
                    }
                    else
                    {
                        //current package expired or sms get over
                        if (currentSMSPackage.RemainingSMS <= 0 || currentSMSPackage.PackageExpiryDate < CommonMethod.CurrentIndianDateTime())
                        {
                            tbl_CompanySMSPackRenew nextSMSPackage = _db.tbl_CompanySMSPackRenew.Where(x => x.CompanyId == companyId
                                                                    && x.RemainingSMS > 0).OrderBy(z => z.SMSPackageId).Take(1).FirstOrDefault();
                            if (nextSMSPackage == null)
                            {
                                response.IsError = true;
                                response.AddError(ErrorMessage.SMSPackageIsExpired);
                            }
                            else
                            {
                                currentSMSPackage.PackageExpiryDate = CommonMethod.CurrentIndianDateTime().AddMinutes(-1);
                                nextSMSPackage.RenewDate = CommonMethod.CurrentIndianDateTime();
                                nextSMSPackage.PackageExpiryDate = CommonMethod.CurrentIndianDateTime().AddDays(nextSMSPackage.AccessDays);
                                companyObj.CurrentSMSPackageId = Convert.ToInt32(nextSMSPackage.CompanySMSPackRenewId);
                                _db.SaveChanges();

                                clsAdminSession.CurrentSMSPackageId = companyObj.CurrentSMSPackageId.HasValue ? companyObj.CurrentSMSPackageId.Value : 0;
                                activeSMSPackage = nextSMSPackage;
                            }
                        }
                        else
                        {
                            activeSMSPackage = currentSMSPackage;
                        }
                    }

                    if (!response.IsError)
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            string formattedMsg = msg;
                            msg = HttpUtility.UrlEncode(msg);

                            string url = GetSMSUrl().Replace("--MOBILE--", mobileNo).Replace("--MSG--", msg);
                            var json = webClient.DownloadString(url);

                            response.IsError = false;
                            response.Data = json;
                            activeSMSPackage = _db.tbl_CompanySMSPackRenew.Where(x => x.CompanySMSPackRenewId == activeSMSPackage.CompanySMSPackRenewId).FirstOrDefault();
                            activeSMSPackage.RemainingSMS = activeSMSPackage.RemainingSMS - 1;
                            _db.SaveChanges();

                            tbl_SMSLog smsLog = new tbl_SMSLog();
                            smsLog.Message = formattedMsg;
                            smsLog.MobileNo = mobileNo;
                            smsLog.CompanyId = companyId;
                            smsLog.EmployeeId = employeeId;
                            smsLog.EmployeeCode = employeeCode;
                            smsLog.CreatedBy = loggedInUser;
                            smsLog.CreatedDate = CommonMethod.CurrentIndianDateTime();
                            _db.tbl_SMSLog.Add(smsLog);
                            _db.SaveChanges();
                        }
                    }
                }
                else
                {
                    response.IsError = false;
                    response.Data = SendSMSWithoutLog(msg, mobileNo);
                }
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        public static GeneralResponseVM SendEmail(string To, string subject, string body)
        {
            GeneralResponseVM responseVM = new GeneralResponseVM();

            try
            {
                AttendanceSystemEntities _db = new AttendanceSystemEntities();
                tbl_Setting objGensetting = _db.tbl_Setting.FirstOrDefault();

                string environnment = ConfigurationManager.AppSettings["Environment"].ToString();

                string SMTPFromEmailId = "";
                string SMTPHost = "";
                int SMTPPort = 0;
                string SMTPEmail = "";
                string SMTPPassword = "";
                bool SMTPEnableSSL = true;

                if (environnment == "Development")
                {
                    SMTPFromEmailId = ConfigurationManager.AppSettings["SMTPFromEmailId"].ToString();
                    SMTPHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
                    SMTPPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"].ToString());
                    SMTPEmail = ConfigurationManager.AppSettings["SMTPEmail"].ToString();
                    SMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"].ToString();
                    SMTPEnableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPEnableSSL"].ToString());
                }
                else
                {
                    SMTPFromEmailId = objGensetting.SMTPFromEmailId;
                    SMTPHost = objGensetting.SMTPHost;
                    SMTPPort = objGensetting.SMTPPort.Value;
                    SMTPEmail = objGensetting.SMTPEmail;
                    SMTPPassword = objGensetting.SMTPPassword;
                    SMTPEnableSSL = objGensetting.SMTPEnableSSL.Value;
                }

                MailMessage mailMessage = new MailMessage(
                      SMTPFromEmailId, // From field
                      To, // Recipient field
                     subject, // Subject of the email message
                      body // Email message body
                 );

                mailMessage.From = new MailAddress(SMTPFromEmailId, "Contract Book");
                mailMessage.IsBodyHtml = true;

                using (SmtpClient client = new SmtpClient())
                {
                    client.EnableSsl = SMTPEnableSSL == true ? true : false;
                    client.UseDefaultCredentials = false;
                    client.Host = SMTPHost;
                    client.Port = Convert.ToInt32(SMTPPort);
                    client.Credentials = new NetworkCredential(SMTPEmail, SMTPPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mailMessage.IsBodyHtml = true;
                    client.Timeout = 10000;
                    client.Send(mailMessage);

                    responseVM.IsError = false;
                }
            }
            catch (Exception e)
            {
                responseVM.IsError = true;
                responseVM.ErrorMessage = e.Message.ToString();
            }

            return responseVM;
        }
        public static List<SelectListItem> GetCalenderMonthList()
        {
            string[] calenderMonthArr = Enum.GetNames(typeof(CalenderMonths));
            var listCalenderMonth = calenderMonthArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listCalenderMonth
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }

        public static string GetSmsContent(int Id)
        {
            AttendanceSystemEntities _db = new AttendanceSystemEntities();
            var objSms = _db.tbl_SMSContent.Where(o => o.SMSContentId == Id).FirstOrDefault();
            if (objSms != null)
            {
                return objSms.SMSDescription.ToString();
            }
            else
            {
                return "";
            }
        }

        public static string GetCurrentDomain()
        {
            var request = HttpContext.Current.Request;
            return request.Url.Scheme + "://" + request.Url.Authority;
        }

        public static string GetFormatterAmount(Decimal? Amt)
        {
            string ConvertedString = "";
            if (Amt != null)
            {
                double ammountDouble = Convert.ToDouble(Amt);

                bool isInt = ammountDouble % 1 == 0;

                string format = "{0:N}";
                if (isInt)
                    format = "{0:N0}";

                CultureInfo cultureInfo = new CultureInfo("en-IN");
                ConvertedString = string.Format(cultureInfo, format, ammountDouble);
            }
            return ConvertedString;
        }

        public static string InvoiceFinancialYear()
        {
            DateTime today = CurrentIndianDateTime().Date;
            int currYear = today.Month >= (int)CalenderMonths.April ? today.Year : today.Year - 1;
            int nextYear = currYear + 1;
            string invoiceYear = currYear + "-" + nextYear;
            return invoiceYear;
        }

        public static string GetInvoiceContent(InvoiceFieldsVM fields)
        {
            string htmlContent = string.Empty;
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~\\Content\\Invoice.htm");
            string logoPath = System.Web.Hosting.HostingEnvironment.MapPath("~\\Content\\admin-theme\\assets\\img\\invoicelogo.png");
            StreamReader objReader = new StreamReader(path);
            htmlContent = objReader.ReadToEnd();
            htmlContent = Regex.Replace(htmlContent, "##companyname##", fields.CompanyName);
            htmlContent = Regex.Replace(htmlContent, "##invoiceno##", fields.InvoiceNo);
            htmlContent = Regex.Replace(htmlContent, "##customername##", fields.CustomerName);
            htmlContent = Regex.Replace(htmlContent, "##invoicedate##", fields.InvoiceDate);
            htmlContent = Regex.Replace(htmlContent, "##address##", fields.Address);
            htmlContent = Regex.Replace(htmlContent, "##phone##", fields.Phone);
            htmlContent = Regex.Replace(htmlContent, "##gstno##", fields.GstNo);
            htmlContent = Regex.Replace(htmlContent, "##panno##", fields.PanNo);
            htmlContent = Regex.Replace(htmlContent, "##packagename##", fields.PackageName);
            htmlContent = Regex.Replace(htmlContent, "##hsncode##", fields.HsnCode);
            htmlContent = Regex.Replace(htmlContent, "##qty##", fields.Qty.ToString("#.##"));
            htmlContent = Regex.Replace(htmlContent, "##rate##", fields.Rate.ToString("#.##"));
            htmlContent = Regex.Replace(htmlContent, "##gstrate##", fields.GSTRate.ToString("#.##"));
            htmlContent = Regex.Replace(htmlContent, "##cgst##", fields.CGST.ToString("#.##"));
            htmlContent = Regex.Replace(htmlContent, "##sgst##", fields.SGST.ToString("#.##"));
            htmlContent = Regex.Replace(htmlContent, "##igst##", fields.IGST.ToString("#.##"));
            htmlContent = Regex.Replace(htmlContent, "##totalamount##", fields.TotalAmount.ToString("#.##"));
            htmlContent = Regex.Replace(htmlContent, "##logo##", logoPath);
            //\Content\Invoice.htm
            return htmlContent.Replace("\r\n", "");
        }

        public static string GetDurationOfJoining(DateTime? JoiningDate)
        {
            string duration = string.Empty;

            if (JoiningDate != null)
            {
                DateTime currentDate = CurrentIndianDateTime();

                const double ApproxDaysPerMonth = 30.4375;
                const double ApproxDaysPerYear = 365.25;

                int iDays = (currentDate - Convert.ToDateTime(JoiningDate)).Days;

                int iYear = (int)(iDays / ApproxDaysPerYear);
                iDays -= (int)(iYear * ApproxDaysPerYear);

                int iMonths = (int)(iDays / ApproxDaysPerMonth);
                iDays -= (int)(iMonths * ApproxDaysPerMonth);

                // Get years
                if (iYear > 0)
                {
                    duration = "<span style='text-decoration: underline;'>" + iYear + "</span>" + " years";
                }
                else
                {
                    if (iMonths > 0)
                    {
                        duration = "<span style='text-decoration: underline;'>" + iMonths + "</span>" + " months";
                    }
                    else
                    {
                        if (iDays > 0)
                        {
                            duration = "<span style='text-decoration: underline;'>" + iDays + "</span>" + " days";
                        }
                    }
                }

            }

            return duration;
        }

        public static string GetRatingCertificateContent(EmployeeRatingVM objEmployeeRating)
        {
            long companyId = clsAdminSession.CompanyId;
            string companyLogo = System.Web.Hosting.HostingEnvironment.MapPath("~\\Images\\default_image.png");

            string duration = GetDurationOfJoining(objEmployeeRating.EmployeeJoiningDate);
            duration = !string.IsNullOrEmpty(duration) ? " for the last " + duration : "";

            string designation = !string.IsNullOrEmpty(objEmployeeRating.EmployeeDesignation) ? " as a <span style='text-decoration: underline;'>" + objEmployeeRating.EmployeeDesignation + "</span>" : "";

            string description = "This guy has been working with us" + duration + designation + ", His behaviour is very nice towards the company. <br /> He doesn't have any legal offenses. It is very simple and quit in nature.";

            AttendanceSystemEntities _db = new AttendanceSystemEntities();
            tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
            if (objCompany != null && !string.IsNullOrEmpty(objCompany.CompanyLogoImage))
            {
                string domainUrl = ConfigurationManager.AppSettings["DomainUrl"].ToString();
                companyLogo = domainUrl + "/Images/CompanyLogo/" + objCompany.CompanyLogoImage;
            }

            string htmlContent = string.Empty;
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~\\Content\\certificate.htm");
            string logoPath = System.Web.Hosting.HostingEnvironment.MapPath("~\\Content\\blank_certificate.jpg");
            StreamReader objReader = new StreamReader(path);
            htmlContent = objReader.ReadToEnd();

            htmlContent = Regex.Replace(htmlContent, "##name##", objEmployeeRating.EmployeeName);
            htmlContent = Regex.Replace(htmlContent, "##companylogo##", companyLogo);
            htmlContent = Regex.Replace(htmlContent, "##behave##", GetFormatterAmount(objEmployeeRating.BehaviourRate).ToString() + "/10");
            htmlContent = Regex.Replace(htmlContent, "##Reglr##", GetFormatterAmount(objEmployeeRating.RegularityRate).ToString() + "/10");
            htmlContent = Regex.Replace(htmlContent, "##Work##", GetFormatterAmount(objEmployeeRating.WorkRate).ToString() + "/10");
            htmlContent = Regex.Replace(htmlContent, "##logo##", logoPath);
            htmlContent = Regex.Replace(htmlContent, "##description##", description);

            return htmlContent.Replace("\r\n", "");
        }

        public static List<SelectListItem> GetDistrictListByStateId(long StateId)
        {
            AttendanceSystemEntities _db = new AttendanceSystemEntities();
            List<SelectListItem> lstDistricts = new List<SelectListItem>();

            List<SelectListItem> lst = (from st in _db.tbl_District
                                        where !st.IsDeleted && st.IsActive
                                        && st.StateId == StateId
                                        select new SelectListItem
                                        {
                                            Text = st.DistrictName,
                                            Value = st.DistrictId.ToString()
                                        }).OrderBy(x => x.Text).ToList();

            lstDistricts = lst != null ? lst : lstDistricts;

            return lstDistricts;
        }

        public static List<SelectListItem> GetStateListOfIndia()
        {
            AttendanceSystemEntities _db = new AttendanceSystemEntities();
            List<SelectListItem> lstStates = new List<SelectListItem>();

            List<SelectListItem> lst = (from st in _db.tbl_State
                                        where !st.IsDeleted && st.IsActive
                                        select new SelectListItem
                                        {
                                            Text = st.StateName,
                                            Value = st.StateId.ToString()
                                        }).OrderBy(x => x.Text).ToList();

            lstStates = lst != null ? lst : lstStates;

            return lstStates;
        }

        public static string GetCurrentFinancialYear()
        {
            DateTime today = CurrentIndianDateTime();

            int CurrentYear = today.Year;
            int PreviousYear = today.Year - 1;
            int NextYear = today.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (today.Month > 3)
                FinYear = CurYear + "-" + NexYear;
            else
                FinYear = PreYear + "-" + CurYear;

            return FinYear.Trim();
        }

        public static long? GetFinancialYearId()
        {
            long? financialId = null;

            try
            {
                string strFinancialYear = GetCurrentFinancialYear();
                if (!string.IsNullOrEmpty(strFinancialYear))
                {
                    AttendanceSystemEntities _db = new AttendanceSystemEntities();
                    var objFinancialYear = _db.mst_FinancialYear.Where(o => o.FinancialYearText == strFinancialYear).FirstOrDefault();
                    if (objFinancialYear != null)
                    {
                        financialId = objFinancialYear.FinancialYearId;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return financialId;
        }

        public static long? GetFinancialYearIdFromDate(DateTime date)
        {
            long? financialId = null;

            try
            {
                int CurrentYear = date.Year;
                int PreviousYear = date.Year - 1;
                int NextYear = date.Year + 1;
                string PreYear = PreviousYear.ToString();
                string NexYear = NextYear.ToString();
                string CurYear = CurrentYear.ToString();
                string strFinancialYear = null;

                if (date.Month > 3)
                    strFinancialYear = CurYear + "-" + NexYear;
                else
                    strFinancialYear = PreYear + "-" + CurYear;

                if (!string.IsNullOrEmpty(strFinancialYear))
                {
                    AttendanceSystemEntities _db = new AttendanceSystemEntities();
                    var objFinancialYear = _db.mst_FinancialYear.Where(o => o.FinancialYearText == strFinancialYear).FirstOrDefault();
                    if (objFinancialYear != null)
                    {
                        financialId = objFinancialYear.FinancialYearId;
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return financialId;
        }

        public static List<SelectListItem> GetFinancialYearList()
        {
            AttendanceSystemEntities _db = new AttendanceSystemEntities();
            List<SelectListItem> lst = (from yr in _db.mst_FinancialYear
                                        select new SelectListItem
                                        {
                                            Text = yr.FinancialYearText,
                                            Value = yr.FinancialYearId.ToString()
                                        }).ToList();
            return lst;
        }

    }
}