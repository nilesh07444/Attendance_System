﻿using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace AttendanceSystem
{
    public static class CommonMethod
    {
        public static string ConvertFromUTC(DateTime? utcDateTime)
        {
            if (utcDateTime == null)
                return "";

            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime dateTimeAsTimeZone = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(utcDateTime), nzTimeZone);
            string dt = dateTimeAsTimeZone.ToString("dd/MM/yyyy hh:mm tt");
            return dt;
        }

        public static string ConvertFromUTCNew(DateTime? utcDateTime)
        {
            if (utcDateTime == null)
                return "";

            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime dateTimeAsTimeZone = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(utcDateTime), nzTimeZone);
            string dt = dateTimeAsTimeZone.ToString("dd-MMM-yyyy hh:mm tt");
            return dt;
        }

        public static string ConvertFromUTCOnlyDate(DateTime? utcDateTime)
        {
            if (utcDateTime == null)
                return "";

            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime dateTimeAsTimeZone = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(utcDateTime), nzTimeZone);
            string dt = dateTimeAsTimeZone.ToString("dd/MM/yyyy");
            return dt;
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

        //public static string GetRandomReferralCode(int length)
        //{
        //    Random random = new Random();
        //    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //    string randomReferralCode = new string(chars.Select(c => chars[random.Next(chars.Length)]).Take(length).ToArray());

        //    krupagallarydbEntities _db = new krupagallarydbEntities();
        //    bool existCode = _db.tbl_ClientUsers.Where(x => x.OwnReferralCode == randomReferralCode).Any();

        //    if (existCode)
        //    {
        //        GetRandomReferralCode(length);
        //    }

        //    return randomReferralCode;
        //}

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
                filepath = filepath + "\\" + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
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

        public static string SuperAdminSendSMS(string msg, string mobileNo, long employeeId)
        {
            using (WebClient webClient = new WebClient())
            {
                msg = HttpUtility.UrlEncode(msg);
                string url = GetSMSUrl().Replace("--MOBILE--", mobileNo).Replace("--MSG--", msg);
                var json = webClient.DownloadString(url);

                AttendanceSystemEntities _db = new AttendanceSystemEntities();
                tbl_SMSLog smsLog = new tbl_SMSLog();
                smsLog.Message = msg;
                smsLog.MobileNo = mobileNo;
                smsLog.CreatedBy = employeeId;
                smsLog.CreatedDate = DateTime.UtcNow;
                _db.tbl_SMSLog.Add(smsLog);
                _db.SaveChanges();
                return json;
            }
        }

        public static ResponseDataModel<string> SendSMS(string msg, string mobileNo, long companyId, long employeeId, bool isTrialMode = false)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {
                if (!isTrialMode)
                {
                    AttendanceSystemEntities _db = new AttendanceSystemEntities();
                    tbl_CompanySMSPackRenew activeSMSPackage = null;
                    tbl_CompanySMSPackRenew currentSMSPackage = _db.tbl_CompanySMSPackRenew.Where(x => x.CompanyId == companyId
                    && x.RenewDate <= DateTime.Now
                    && x.PackageExpiryDate > DateTime.Now).FirstOrDefault();

                    //could not found any active package 
                    if (currentSMSPackage == null)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.SMSPackageIsExpired);
                    }
                    else
                    {
                        //current package expired or sms get over
                        if (currentSMSPackage.RemainingSMS == 0 || currentSMSPackage.PackageExpiryDate < DateTime.Now)
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
                                currentSMSPackage.PackageExpiryDate = DateTime.Now.AddMinutes(-1);
                                nextSMSPackage.RenewDate = DateTime.Now;
                                nextSMSPackage.PackageExpiryDate = DateTime.Now.AddDays(nextSMSPackage.AccessDays);
                                _db.SaveChanges();
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
                            msg = HttpUtility.UrlEncode(msg);
                            string url = GetSMSUrl().Replace("--MOBILE--", mobileNo).Replace("--MSG--", msg);
                            var json = webClient.DownloadString(url);
                            response.IsError = false;
                            response.Data = json;
                            activeSMSPackage = _db.tbl_CompanySMSPackRenew.Where(x => x.SMSPackageId == activeSMSPackage.SMSPackageId).FirstOrDefault();
                            activeSMSPackage.RemainingSMS = activeSMSPackage.RemainingSMS - 1;
                            _db.SaveChanges();

                            tbl_SMSLog smsLog = new tbl_SMSLog();
                            smsLog.Message = msg;
                            smsLog.MobileNo = mobileNo;
                            smsLog.CreatedBy = employeeId;
                            smsLog.CreatedDate = DateTime.UtcNow;
                            _db.tbl_SMSLog.Add(smsLog);
                            _db.SaveChanges();
                        }
                    }
                }
                else
                {
                    response.IsError = false;
                    response.Data = SuperAdminSendSMS(msg, mobileNo, employeeId);
                }
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

    }
}