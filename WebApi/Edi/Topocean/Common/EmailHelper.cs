using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TB_WEB.CommonLibrary.Date;
using TB_WEB.CommonLibrary.Encrypt;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;
using WebApi.Edi.Topocean.Edi_Impl;
using WebApi.Edi.Topocean.EdiModels.Common;

namespace WebApi.Edi.Common
{
    public class EmailHelper
    {
        // Track whether Dispose has been called.
        private bool disposed = false;
        private string SMTP_SERVER = string.Empty;
        private int SMTP_PORT = 0;
        private string DEFAULT_SENDER = string.Empty;
        private string DEFAULT_BCC = string.Empty;

        public EmailHelper()
        {
            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("TBS_SMTPSERVER"))
            {
                SMTP_SERVER = System.Configuration.ConfigurationManager.AppSettings["TBS_SMTPSERVER"].ToString();
            }

            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("TBS_SMTPPORT"))
            {
                SMTP_PORT = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["TBS_SMTPPORT"].ToString());
            }

            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("TBS_DEFAULT_SENDER"))
            {
                DEFAULT_SENDER = System.Configuration.ConfigurationManager.AppSettings["TBS_DEFAULT_SENDER"].ToString();
            }

            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("TBS_DEFAULT_BCC"))
            {
                DEFAULT_BCC = System.Configuration.ConfigurationManager.AppSettings["TBS_DEFAULT_BCC"].ToString();
            }

        }

        public EmailHelper(string smptStr, string smtpport, string senderStr)
        {
            SMTP_SERVER = smptStr;
            SMTP_PORT = Int32.Parse(smtpport);
            DEFAULT_SENDER = senderStr;
        }

        public bool SendMail(string mailSubject, string mailBody, string mailSender, string[] mailReciever, string[] mailCC = null, Attachment[] attachmentList = null)
        {
            try
            {
                using (SmtpClient client = new SmtpClient(SMTP_SERVER, SMTP_PORT))
                {
                    using (MailMessage message = new MailMessage())
                    {
                        message.IsBodyHtml = true;

                        //mail subject
                        message.SubjectEncoding = Encoding.UTF8;
                        message.Subject = mailSubject;

                        //mail body
                        message.BodyEncoding = Encoding.UTF8;

                        var result = PreMailer.Net.PreMailer.MoveCssInline(mailBody);
                        mailBody = result.Html;

                        message.Body = mailBody;


                        //sender
                        message.From = new MailAddress(mailSender);

                        //reciever
                        foreach (string item in mailReciever)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                message.To.Add(new MailAddress(item));
                            }
                        }

                        //cc
                        if (mailCC != null)
                        {
                            foreach (string item in mailCC)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    message.CC.Add(new MailAddress(item));
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(DEFAULT_BCC))
                        {
                            message.Bcc.Add(new MailAddress(DEFAULT_BCC));
                        }

                        if (attachmentList != null && attachmentList.Count() > 0)
                        {
                            foreach (Attachment item in attachmentList)
                            {
                                message.Attachments.Add(item);
                            }
                        }

                        //client.Credentials = new System.Net.NetworkCredential("username", "password");
                        //client.EnableSsl = true;
                        client.Send(message);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return false;
            }
        }

        public bool SendMailViaAPI(string mailSubject, string mailBody, string mailReciever, string[] attachmentList)
        {
            try
            {
                DataTable tmpDT = new DataTable();
                string base64Mail = string.Empty;

                tmpDT.Columns.Add(new DataColumn() { ColumnName = "source", DataType = Type.GetType("System.String") }); //default: base64
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "source_content", DataType = Type.GetType("System.String") }); //base64 encode whole mail JSON
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "destination", DataType = Type.GetType("System.String") });//default: email
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "destination_content", DataType = Type.GetType("System.String") }); //Reciever

                tmpDT.Columns.Add(new DataColumn() { ColumnName = "from", DataType = Type.GetType("System.String") }); //email sender
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "subject", DataType = Type.GetType("System.String") }); //email subject
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "body", DataType = Type.GetType("System.String") }); //email body
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "type", DataType = Type.GetType("System.String") }); //email type: default: html

                tmpDT.Columns.Add(new DataColumn() { ColumnName = "sync", DataType = Type.GetType("System.Int32") }); //1= sync; 0=async

                tmpDT.Rows.Add(new Object[] { "base64", base64Mail, "email", mailReciever, DEFAULT_SENDER, mailSubject, mailBody, "html", 1 });

                //Get the JSON via FileTransfer_Imp class
                FileTransfer_Imp fileTransfer_Imp = new FileTransfer_Imp();
                fileTransfer_Imp.init(tmpDT);

                //attach files
                if (attachmentList != null && attachmentList.Length > 0)
                {
                    foreach (string item in attachmentList)
                    {
                        //upload attach file to S3
                        string[] uploadedFile = DocumentHelper.PODownloadDoc2S3(item);
                        if (uploadedFile[0].ToUpper() != "SUCCESS")
                        {
                            throw (new Exception("Fail to upload attach file to S3"));
                        }

                        //add attach file to API mail object
                        fileTransfer_Imp.AddMailAttach(uploadedFile[2], uploadedFile[3], uploadedFile[1]);
                    }
                }
                string postJSON = fileTransfer_Imp.serialize();

                // Authentication
                AuthenticationModel authenticationModel = new AuthenticationModel(
                        new AuthenticationEntity() { usr = BaseCont.usr, psd = new Decryption().DecryptDBConnectionString(BaseCont.psd) });

                //call /file-transfer API via FileTransferResponse_Imp
                FileTransferResponse_Imp fileTransferResponse_Imp = new FileTransferResponse_Imp();
                fileTransferResponse_Imp.init(
                        new EDIPostEntity() { queryParams = null, body = postJSON, apiUrl = BaseCont.ApiUrl + "/file-transfer" }
                        , authenticationModel);

                fileTransferResponse_Imp.Post();

                //get the response object
                FileTransferResponse<FileTransfer_PayloadItem> respObj = fileTransferResponse_Imp.LoadBaseResponse<FileTransferResponse<FileTransfer_PayloadItem>>();

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return false;
            }
        }

        public bool SendMailViaAPI(string mailSubject, string mailBody, string mailReciever,Dictionary<string, MemoryStream> attachmentList)
        {
            try
            {
                DataTable tmpDT = new DataTable();
                string base64Mail = string.Empty;

                tmpDT.Columns.Add(new DataColumn() { ColumnName = "source", DataType = Type.GetType("System.String") }); //default: base64
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "source_content", DataType = Type.GetType("System.String") }); //base64 encode whole mail JSON
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "destination", DataType = Type.GetType("System.String") });//default: email
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "destination_content", DataType = Type.GetType("System.String") }); //Reciever

                tmpDT.Columns.Add(new DataColumn() { ColumnName = "from", DataType = Type.GetType("System.String") }); //email sender
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "subject", DataType = Type.GetType("System.String") }); //email subject
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "body", DataType = Type.GetType("System.String") }); //email body
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "type", DataType = Type.GetType("System.String") }); //email type: default: html

                tmpDT.Columns.Add(new DataColumn() { ColumnName = "sync", DataType = Type.GetType("System.Int32") }); //1= sync; 0=async

                tmpDT.Rows.Add(new Object[] { "base64", base64Mail, "email", mailReciever, DEFAULT_SENDER, mailSubject, mailBody, "html", 1 });

                //Get the JSON via FileTransfer_Imp class
                FileTransfer_Imp fileTransfer_Imp = new FileTransfer_Imp();
                fileTransfer_Imp.init(tmpDT);

                //attach files
                if (attachmentList != null)
                {
                    Dictionary<string, MemoryStream> attachList = attachmentList;
                    foreach (KeyValuePair<string, MemoryStream> kvp in attachList)
                    {
                        string[] uploadedFile = DocumentHelper.PODownloadDoc2S3(kvp.Value, kvp.Key);

                        if (uploadedFile[0].ToUpper() != "SUCCESS")
                        {
                            throw (new Exception("Fail to upload attach file to S3"));
                        }

                        //add attach file to API mail object
                        fileTransfer_Imp.AddMailAttach(uploadedFile[2], uploadedFile[3], uploadedFile[1]);
                    }
                }
                string postJSON = fileTransfer_Imp.serialize();

                // Authentication
                AuthenticationModel authenticationModel = new AuthenticationModel(
                        new AuthenticationEntity() { usr = BaseCont.usr, psd = new Decryption().DecryptDBConnectionString(BaseCont.psd) });

                //call /file-transfer API via FileTransferResponse_Imp
                FileTransferResponse_Imp fileTransferResponse_Imp = new FileTransferResponse_Imp();
                fileTransferResponse_Imp.init(
                        new EDIPostEntity() { queryParams = null, body = postJSON, apiUrl = BaseCont.ApiUrl + "/file-transfer" }
                        , authenticationModel);

                fileTransferResponse_Imp.Post();

                //get the response object
                FileTransferResponse<FileTransfer_PayloadItem> respObj = fileTransferResponse_Imp.LoadBaseResponse<FileTransferResponse<FileTransfer_PayloadItem>>();

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return false;
            }
        }

        public bool SendSystemMail(string mailSubject, string mailBody, string[] mailReciever, string[] mailCC = null, Attachment[] attachmentList = null)
        {
            return SendMail(mailSubject, mailBody, DEFAULT_SENDER, mailReciever, mailCC, attachmentList);
        }

        public string BuildMailBody(string messageStr, string templateType)
        {
            try
            {
                string templatePath = string.Empty;
                string bodyContent = string.Empty;
                string cssContent = string.Empty;

                switch (templateType.ToUpper())
                {
                    default:
                        templatePath = "~/FormTemplate/MailBody.tpl";
                        break;
                }

                templatePath = HttpContext.Current.Server.MapPath(templatePath);
                if (!string.IsNullOrEmpty(templatePath))
                {
                    using (StreamReader streamReader = new StreamReader(templatePath, Encoding.UTF8))
                    {
                        bodyContent = streamReader.ReadToEnd();
                    }
                }

                bodyContent = bodyContent.Replace("/*[[CSS]]*/", cssContent);
                bodyContent = bodyContent.Replace("<!--[[Body]]-->", messageStr);

                return bodyContent;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return ex.ToString();
            }
        }

        public string BuildMailTest()
        {
            try
            {
                string templatePath = string.Empty;
                string bodyContent = string.Empty;
                string cssContent = string.Empty;
                templatePath = "~/Test.html";
                templatePath = HttpContext.Current.Server.MapPath(templatePath);
                if (!string.IsNullOrEmpty(templatePath))
                {
                    using (StreamReader streamReader = new StreamReader(templatePath, Encoding.UTF8))
                    {
                        bodyContent = streamReader.ReadToEnd();
                    }
                }

                return bodyContent;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return ex.ToString();
            }
        }

        private string LoadTemplate(string templateType, string templatePath = "")
        {
            try
            {
                string templateStr = string.Empty;
                if (!string.IsNullOrEmpty(templateType) && string.IsNullOrEmpty(templatePath))
                {
                    switch (templateType.ToUpper())
                    {
                        case "COMMONBOOKINGDETAIL":
                            templatePath = "~/FormTemplate/CommonBookingDetail.tpl";
                            break;

                        case "COMMONBOOKINGDETAILV3":
                            templatePath = "~/FormTemplate/CommonBookingDetailV3.tpl";
                            break;

                        case "COMMONBOOKINGDETAILV2":
                            templatePath = "~/FormTemplate/CommonBookingDetailV2.tpl";
                            break;

                        case "COMMONBOOKINGROUTE":
                            templatePath = "~/FormTemplate/CommonBookingRoute.tpl";
                            break;

                        case "COMMONBOOKINGROUTE_DTL":
                            templatePath = "~/FormTemplate/CommonBookingRouteDtl.tpl";
                            break;

                        case "COMMONBOOKINGROUTE2":
                            templatePath = "~/FormTemplate/CommonBookingRoute2.tpl";
                            break;

                        case "COMMONBOOKINGROUTE_DTL2":
                            templatePath = "~/FormTemplate/CommonBookingRouteDtl2.tpl";
                            break;

                        case "SHIPPINGORDER":
                            templatePath = "~/FormTemplate/ShippingOrder.tpl";
                            break;

                        case "MBLFORM":
                            templatePath = "~/FormTemplate/MBLForm.tpl";
                            break;

                        case "PREALERT":
                            templatePath = "~/FormTemplate/PreAlert.tpl";
                            break;

                        case "PREALERT_AIR":
                            templatePath = "~/FormTemplate/PreAlert_Air.tpl";
                            break;

                        default:
                            templatePath = "~/FormTemplate/MailBody.tpl";
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(templatePath))
                {
                    templatePath = HttpContext.Current.Server.MapPath(templatePath);
                    if (!string.IsNullOrEmpty(templatePath))
                    {
                        using (StreamReader streamReader = new StreamReader(templatePath, Encoding.UTF8))
                        {
                            templateStr = streamReader.ReadToEnd();
                        }
                    }
                }

                return templateStr;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return "";
            }
        }

        private string LoadCommonBookingSection(string bookingReqID, string mailContent)
        {
            string retVal = string.Empty;
            try
            {
                string tmpContent = string.Empty;

                if (string.IsNullOrEmpty(bookingReqID))
                {
                    throw (new Exception("Missing Booking ID"));
                }

                if (mailContent.IndexOf("<!--[[COMMON_BOOKING_DETAIL]]-->") >= 0)
                {
                    tmpContent = this.LoadCommonBookingDetail(bookingReqID);
                    mailContent = mailContent.Replace("<!--[[COMMON_BOOKING_DETAIL]]-->", tmpContent);
                }

                if (mailContent.IndexOf("<!--[[COMMON_BOOKING_DETAILV3]]-->") >= 0)
                {
                    tmpContent = this.LoadCommonBookingDetailV3(bookingReqID);
                    mailContent = mailContent.Replace("<!--[[COMMON_BOOKING_DETAILV3]]-->", tmpContent);
                }

                if (mailContent.IndexOf("<!--[[COMMON_BOOKING_DETAILV2]]-->") >= 0)
                {
                    tmpContent = this.LoadCommonBookingDetailV2(bookingReqID);
                    mailContent = mailContent.Replace("<!--[[COMMON_BOOKING_DETAILV2]]-->", tmpContent);
                }

                if (mailContent.IndexOf("<!--[[COMMON_BOOKING_ROUTE]]-->") >= 0)
                {
                    tmpContent = this.LoadCommonBookingRoute(bookingReqID);
                    mailContent = mailContent.Replace("<!--[[COMMON_BOOKING_ROUTE]]-->", tmpContent);
                }

                if (mailContent.IndexOf("<!--[[COMMON_BOOKING_ROUTE2]]-->") >= 0)
                {
                    tmpContent = this.LoadCommonBookingRoute(bookingReqID, 2);
                    mailContent = mailContent.Replace("<!--[[COMMON_BOOKING_ROUTE2]]-->", tmpContent);
                }

                retVal = mailContent;
            }
            catch (Exception ex)
            {
                retVal = string.Empty;

            }

            return retVal;
        }

        private string LoadCommonBookingDetail(string bookingReqID)
        {
            string retVal = string.Empty;
            try
            {
                DataTable dt;
                DataRow dr;
                string sql = string.Empty;
                string mailContent = string.Empty;

                if (string.IsNullOrEmpty(bookingReqID))
                {
                    throw (new Exception("Missing Booking ID"));
                }

                //Get mail template
                mailContent = this.LoadTemplate("COMMONBOOKINGDETAIL");
                if (string.IsNullOrEmpty(mailContent))
                {
                    throw (new Exception("Blank Template"));
                }

                sql = "select BookingReqID, ConsolidationID, BookingDate, OriginOffice, Dest, CNEE, Vendor, ";
                sql += "MBL, HBL, DestRamp as Ramp, IPIETA as RampETA, FinalDest, D2DETA as FinalETA, ";
                sql += "WB_Status as Status, WBApprovalStatus as ApprovalStatus, POReadyDate, ";
                sql += "IsTBS, DeliveryType, CarrierDest, CarrierDestType, SICutOffDate as MultiCutoff, ";
                sql += "CYCutOffDate as CYCutoff, dbo.udf_GetCTNRTypeListFromID(BookingReqID) as CTNRList  ";
                sql += "from POTracing where ISNULL(BookingReqID, '') = '{0}'";
                sql = string.Format(sql, bookingReqID);

                using (DBHelper dbHelper = new DBHelper())
                {
                    dt = dbHelper.ExecDataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (dc.DataType == System.Type.GetType("System.DateTime") || dc.DataType == System.Type.GetType("System.TimeSpan"))
                            {
                                mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName), DateFormatHelper.DisplayDate((DBHelper.GetDataColumnValue<DateTime>(dt.Rows[0], dc.ColumnName, DateTime.MinValue))));
                            }
                            else
                            {
                                mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName), DBHelper.GetDataColumnValue<string>(dt.Rows[0], dc.ColumnName, ""));
                            }
                        }

                        if (DBHelper.GetDataColumnValue<string>(dt.Rows[0], "IsTBS", "") == "Y")
                        {
                            mailContent = mailContent.Replace("<!--[[C_FinalDest_Title]]-->", "Carrier Dest");
                            mailContent = mailContent.Replace("<!--[[C_FinalDest]]-->", DBHelper.GetDataColumnValue<string>(dt.Rows[0], "CarrierDest", ""));
                        }
                        else
                        {
                            mailContent = mailContent.Replace("<!--[[C_FinalDest_Title]]-->", "Final Dest");
                            mailContent = mailContent.Replace("<!--[[C_FinalDest]]-->", DBHelper.GetDataColumnValue<string>(dt.Rows[0], "FinalDest", ""));
                        }
                    }
                }

                retVal = mailContent;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                retVal = string.Empty;
            }

            return retVal;
        }

        private string LoadCommonBookingDetailV3(string bookingReqID)
        {
            string retVal = string.Empty;
            try
            {
                DataTable dt;
                DataRow dr;
                string sql = string.Empty;
                string mailContent = string.Empty;

                if (string.IsNullOrEmpty(bookingReqID))
                {
                    throw (new Exception("Missing Booking ID"));
                }

                //Get mail template
                mailContent = this.LoadTemplate("COMMONBOOKINGDETAILV3");
                if (string.IsNullOrEmpty(mailContent))
                {
                    throw (new Exception("Blank Template"));
                }

                sql = "select BookingConfirmation, Term, (Select top 1 SCAC from Carrier where POTracing.Carrier = Carrier.CarrierName) as CarrierSCAC, (case when BookingContractType = 'TOPOCEAN' Then 'TOPO' Else ISNULL((Select top 1 SCAC from Carrier where POTracing.FreightForwarder = Carrier.CarrierName), '') end) as TopoceanSCAC, Description as ActualCommodity, ";
                sql += "BookingReqID, ConsolidationID, BookingDate, OriginOffice, Dest, CNEE, Vendor, ";
                sql += "MBL, HBL, DestRamp as Ramp, IPIETA as RampETA, FinalDest, D2DETA as FinalETA, ";
                sql += "WB_Status as Status, WBApprovalStatus as ApprovalStatus, POReadyDate, ";
                sql += "IsTBS, DeliveryType, (CASE when CarrierDestType = 'PORT' then (select top 1 CONCAT(PortName, ', ', ISNULL(State,Country)) from Port where PortAbbr = CarrierDest) ELSE CarrierDest END) as CarrierDest, CarrierDestType, SICutOffDate as MultiCutoff, ";
                sql += "CYCutOffDate as CYCutoff, dbo.udf_GetCTNRTypeListFromID(BookingReqID) as CTNRList, ";
                sql += "Term, (Select top 1 SCAC from Carrier where POTracing.Carrier = Carrier.CarrierName) as CarrierSCAC, (case when BookingContractType = 'TOPOCEAN' Then 'TOPO' Else ISNULL((Select top 1 SCAC from Carrier where POTracing.FreightForwarder = Carrier.CarrierName), '') end) as TopoceanSCAC, Commodity2 as CarrierCommodity,  ";
                sql += "CNEECarriercontractNo as CarrierContract, (select SUM(WB_CNTR_POLine.CartonQty) from WB_CNTR_POLine where WB_CNTR_POLine.BookingReqID = POTracing.BookingReqID) as TotalCarton, ";
                sql += "(select SUM(WB_CNTR_POLine.Weight) from WB_CNTR_POLine where WB_CNTR_POLine.BookingReqID = POTracing.BookingReqID) as TotalWeight, (select SUM(WB_CNTR_POLine.CBM) from WB_CNTR_POLine where WB_CNTR_POLine.BookingReqID = POTracing.BookingReqID) as TotalCBM ";
                sql += "from POTracing where ISNULL(BookingReqID, '') = '{0}'";
                sql = string.Format(sql, bookingReqID);

                using (DBHelper dbHelper = new DBHelper())
                {
                    dt = dbHelper.ExecDataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (dc.DataType == System.Type.GetType("System.DateTime") || dc.DataType == System.Type.GetType("System.TimeSpan"))
                            {
                                mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName), DateFormatHelper.DisplayDate((DBHelper.GetDataColumnValue<DateTime>(dt.Rows[0], dc.ColumnName, DateTime.MinValue))));
                            }
                            else if (dc.ColumnName == "TotalWeight" || dc.ColumnName == "TotalCBM")
                            {
                                mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName), DBHelper.GetDataColumnValue<Decimal>(dt.Rows[0], dc.ColumnName, 0).ToString("0.##"));
                            }
                            else
                            {
                                mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName), DBHelper.GetDataColumnValue<string>(dt.Rows[0], dc.ColumnName, ""));
                            }
                        }

                        if (DBHelper.GetDataColumnValue<string>(dt.Rows[0], "CarrierDestType", "") != "" || DBHelper.GetDataColumnValue<string>(dt.Rows[0], "CarrierDest", "") != "")
                        {
                            mailContent = mailContent.Replace("<!--[[CarrierDestAndType]]-->", DBHelper.GetDataColumnValue<string>(dt.Rows[0], "CarrierDestType", "") + " / " + DBHelper.GetDataColumnValue<string>(dt.Rows[0], "CarrierDest", ""));
                        }
                    }
                }
                retVal = mailContent;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                retVal = string.Empty;
            }

            return retVal;
        }


        private string LoadCommonBookingDetailV2(string bookingReqID)
        {
            string retVal = string.Empty;
            try
            {
                DataTable dt;
                DataRow dr;
                string sql = string.Empty;
                string mailContent = string.Empty;

                if (string.IsNullOrEmpty(bookingReqID))
                {
                    throw (new Exception("Missing Booking ID"));
                }

                //Get mail template
                mailContent = this.LoadTemplate("COMMONBOOKINGDETAILV2");
                if (string.IsNullOrEmpty(mailContent))
                {
                    throw (new Exception("Blank Template"));
                }

                sql = "select BookingConfirmation, Term, (Select top 1 SCAC from Carrier where POTracing.Carrier = Carrier.CarrierName) as CarrierSCAC, (case when BookingContractType = 'TOPOCEAN' Then 'TOPO' Else ISNULL((Select top 1 SCAC from Carrier where POTracing.FreightForwarder = Carrier.CarrierName), '') end) as TopoceanSCAC, Description as ActualCommodity, ";
                sql += "BookingReqID, ConsolidationID, BookingDate, OriginOffice, Dest, CNEE, Vendor, ";
                sql += "MBL, HBL, DestRamp as Ramp, IPIETA as RampETA, FinalDest, D2DETA as FinalETA, ";
                sql += "WB_Status as Status, WBApprovalStatus as ApprovalStatus, POReadyDate, ";
                sql += "IsTBS, DeliveryType, CarrierDest, CarrierDestType, SICutOffDate as MultiCutoff, ";
                sql += "CYCutOffDate as CYCutoff, dbo.udf_GetCTNRTypeListFromID(BookingReqID) as CTNRList  ";
                sql += "from POTracing where ISNULL(BookingReqID, '') = '{0}'";
                sql = string.Format(sql, bookingReqID);

                using (DBHelper dbHelper = new DBHelper())
                {
                    dt = dbHelper.ExecDataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (dc.DataType == System.Type.GetType("System.DateTime") || dc.DataType == System.Type.GetType("System.TimeSpan"))
                            {
                                mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName), DateFormatHelper.DisplayDate((DBHelper.GetDataColumnValue<DateTime>(dt.Rows[0], dc.ColumnName, DateTime.MinValue))));
                            }
                            else
                            {
                                mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName), DBHelper.GetDataColumnValue<string>(dt.Rows[0], dc.ColumnName, ""));
                            }
                        }

                        if (DBHelper.GetDataColumnValue<string>(dt.Rows[0], "IsTBS", "") == "Y")
                        {
                            mailContent = mailContent.Replace("<!--[[C_FinalDest_Title]]-->", "Carrier Dest");
                            mailContent = mailContent.Replace("<!--[[C_FinalDest]]-->", DBHelper.GetDataColumnValue<string>(dt.Rows[0], "CarrierDest", ""));
                        }
                        else
                        {
                            mailContent = mailContent.Replace("<!--[[C_FinalDest_Title]]-->", "Final Dest");
                            mailContent = mailContent.Replace("<!--[[C_FinalDest]]-->", DBHelper.GetDataColumnValue<string>(dt.Rows[0], "FinalDest", ""));
                        }
                    }
                }

                retVal = mailContent;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                retVal = string.Empty;
            }

            return retVal;
        }


        private string LoadCommonBookingRoute(string bookingReqID, int subType = 0)
        {
            string retVal = string.Empty;
            try
            {
                DataTable dt;
                DataRow dr;
                string sql = string.Empty;
                string mailContent = string.Empty;
                string mailContentSub = string.Empty;
                string mailContentSubTmp = string.Empty;
                string tmpStr = string.Empty;

                if (string.IsNullOrEmpty(bookingReqID))
                {
                    throw (new Exception("Missing Booking ID"));
                }

                //Get mail template
                switch (subType)
                {
                    case 2:
                        mailContent = this.LoadTemplate("COMMONBOOKINGROUTE2");
                        break;

                    default:
                        mailContent = this.LoadTemplate("COMMONBOOKINGROUTE");
                        break;
                }

                if (string.IsNullOrEmpty(mailContent))
                {
                    throw (new Exception("Blank Template"));
                }


                switch (subType)
                {
                    case 2:
                        mailContentSubTmp = this.LoadTemplate("COMMONBOOKINGROUTE_DTL2");
                        break;

                    default:
                        mailContentSubTmp = this.LoadTemplate("COMMONBOOKINGROUTE_DTL");
                        break;
                }

                if (string.IsNullOrEmpty(mailContentSubTmp))
                {
                    throw (new Exception("Blank Template"));
                }

                sql = "select *, ISNULL((select top 1 Port.PortAbbr from Port where Port.LoadPort = 'Y' and CONCAT(ISNULL(Port.ISOCountryCode, ''), ";
                sql += " ISNULL(Port.ISOPortCode, '')) = TBS_Route.LoadPortUN order by Port.PortName,Port.State), '') as WBILoadPort, ";
                sql += " ISNULL((select top 1 Port.PortAbbr from Port where Port.DischPort='Y' and CONCAT(ISNULL(Port.ISOCountryCode, ''), ISNULL(Port.ISOPortCode, '')) = TBS_Route.DiscPortUN order by Port.PortName,Port.State), '') as WBIDiscPort, ";
                sql += " ISNULL((Select top 1 Carrier.CarrierName From Carrier where Carrier.TransportationMode like '%SEA%' AND Carrier.SCAC = TBS_Route.Carrier Order By Carrier.CarrierName), '') as WBICarrier, ";
                sql += " CONVERT(varchar, ETD, 101) as ETDFormat, CONVERT(varchar, ETA, 101) as ETAFormat, ";
                sql += " ETD, ETA, ";
                sql += " CONVERT(varchar, CYClosingDate, 101) as CYClosingDateFormat, CONVERT(varchar, CYCloseDate, 101) as CYCloseDateFormat, ";
                sql += " CONVERT(varchar, ATD, 101) as ATDFormat, CONVERT(varchar, ATA, 101) as ATAFormat, CONVERT(varchar, VslOnboard, 101) as VslOnboardFormat, ";
                sql += " CONVERT(varchar, VslOriginalETD, 101) as VslOriginalETDFormat, CONVERT(varchar, VslOriginalETA, 101) as VslOriginalETAFormat ";
                sql += " From TBS_Route where IsActive=1 AND WBIID = '{0}' order by RefId";
                sql = string.Format(sql, bookingReqID);

                using (DBHelper dbHelper = new DBHelper())
                {
                    dt = dbHelper.ExecDataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            tmpStr = mailContentSubTmp;
                            tmpStr = tmpStr.Replace("<!--[[VesselName]]-->", DBHelper.GetDataColumnValue<string>(item, "VesselName", ""));
                            tmpStr = tmpStr.Replace("<!--[[Voyage]]-->", DBHelper.GetDataColumnValue<string>(item, "Voyage", ""));
                            tmpStr = tmpStr.Replace("<!--[[Carrier]]-->", DBHelper.GetDataColumnValue<string>(item, "Carrier", ""));
                            tmpStr = tmpStr.Replace("<!--[[LoadPort]]-->", DBHelper.GetDataColumnValue<string>(item, "LoadPort", ""));
                            tmpStr = tmpStr.Replace("<!--[[ETDFormat]]-->", DateFormatHelper.DisplayDate(DBHelper.GetDataColumnValue<DateTime>(item, "ETD", DateTime.MinValue)));
                            tmpStr = tmpStr.Replace("<!--[[DiscPort]]-->", DBHelper.GetDataColumnValue<string>(item, "DiscPort", ""));
                            tmpStr = tmpStr.Replace("<!--[[ETAFormat]]-->", DateFormatHelper.DisplayDate(DBHelper.GetDataColumnValue<DateTime>(item, "ETA", DateTime.MinValue)));
                            tmpStr = tmpStr.Replace("<!--[[VslSVC]]-->", DBHelper.GetDataColumnValue<string>(item, "VslSVC", ""));

                            mailContentSub += tmpStr;
                        }
                    }
                }
                mailContent = mailContent.Replace("<!--[[ROUTES]]-->", mailContentSub);
                retVal = mailContent;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                retVal = string.Empty;
            }

            return retVal;
        }

        #region send mail via API

        #endregion

        public string ShippingOrderContent(string bookingReqID)
        {
            string retVal = string.Empty;
            try
            {
                DataTable dt;
                DataRow dr;
                string sql = string.Empty;
                string mailContent = string.Empty;

                //Get mail template
                mailContent = this.LoadTemplate("SHIPPINGORDER");

                if (string.IsNullOrEmpty(mailContent))
                {
                    throw (new Exception("Blank Template"));
                }

                //load commmon section
                mailContent = this.LoadCommonBookingSection(bookingReqID, mailContent);

                //loading specify content
                sql = "select  BookingConfirmation, Term, (Select top 1 SCAC from Carrier where POTracing.Carrier = Carrier.CarrierName) as CarrierSCAC, (case when BookingContractType = 'TOPOCEAN' Then 'TOPO' Else ISNULL((Select top 1 SCAC from Carrier where POTracing.FreightForwarder = Carrier.CarrierName), '') end) as TopoceanSCAC, Description as ActualCommodity, ";
                sql += "CFSCutoffDate as CFSCutoff, ShipmentType, ShippingOrder.CustomerReference, ShippingOrder.SORemarks, ShippingOrder.LocalChargeAmt, ShippingOrder.ConsolidatorAddress, Vendor ";
                sql += "from POTracing left join ShippingOrder on POTracing.BookingReqID = ShippingOrder.BookingReqID ";
                sql += "where ISNULL(POTracing.BookingReqID, '') = '{0}' ";
                sql = string.Format(sql, bookingReqID);

                using (DBHelper dbHelper = new DBHelper())
                {
                    dt = dbHelper.ExecDataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {

                            if (dc.DataType == System.Type.GetType("System.DateTime") || dc.DataType == System.Type.GetType("System.TimeSpan"))
                            {
                                mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName), DateFormatHelper.DisplayDate(DBHelper.GetDataColumnValue<DateTime>(dt.Rows[0], dc.ColumnName, DateTime.MinValue)));
                            }
                            else if (dc.ColumnName == "LocalChargeAmt")
                            {
                                if (DBHelper.GetDataColumnValue<Double>(dt.Rows[0], dc.ColumnName, 0) == 0)
                                {
                                    mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName), "-");
                                }
                                else
                                {
                                    mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName), DBHelper.GetDataColumnValue<string>(dt.Rows[0], dc.ColumnName, ""));
                                }
                            }
                            else
                            {
                                mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName), DBHelper.GetDataColumnValue<string>(dt.Rows[0], dc.ColumnName, ""));
                            }
                        }

                        if (DBHelper.GetDataColumnValue<string>(dt.Rows[0], "ShipmentType", "") == "LCL")
                        {
                            mailContent = mailContent.Replace("<!--[[CFSDisplay]]-->", "''");
                        }
                        else
                        {
                            mailContent = mailContent.Replace("<!--[[CFSDisplay]]-->", "none");
                        }
                    }
                }

                mailContent = mailContent.Replace("<!--[[NOW]]-->", DateFormatHelper.DisplayDate(DateTime.Now));

                retVal = mailContent;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                retVal = string.Empty;
            }

            return retVal;
        }

        public string PreAlertContent(string bookingReqID)
        {
            string retVal = string.Empty;
            try
            {
                DataTable dt;
                string sql = string.Empty;
                string mailContent = string.Empty;
                string hblList = string.Empty;
                string mbl = string.Empty;
                string transMode = string.Empty;

                //loading specify content
                sql = "select sub.BookingReqID, sub.MBL, sub.HBL, p.Vessel, p.Voyage, p.P2PATD as ATD, p.P2PETA as ETA, ";
                sql += "(select PortName from Port where PortAbbr = p.LoadPort) as LoadPort, (select PortName from Port where PortAbbr = p.DischPort) as DiscPort, ";
                sql += "p.TransportationMode from POTracing p WITH(NOLOCK) ";
                sql += "inner JOIN POTracing sub WITH(NOLOCK)  on p.MBL = sub.MBL and p.BookingReqID is not NULL and sub.BookingReqID is not NULL and ISNULL(p.IsTBS, '') = ISNULL(sub.IsTBS, '') ";
                sql += "where p.BookingReqID = '{0}' ";
                sql = string.Format(sql, bookingReqID);

                using (DBHelper dbHelper = new DBHelper())
                {
                    dt = dbHelper.ExecDataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        //Get mail template
                        transMode = DBHelperBase.GetDataColumnValue<string>(dt.Rows[0]["TransportationMode"], "");
                        switch (transMode.ToUpper())
                        {
                            case "AIR":
                                mailContent = this.LoadTemplate("PREALERT_AIR");
                                break;

                            default:
                                mailContent = this.LoadTemplate("PREALERT");
                                break;
                        }

                        //load commmon section
                        mailContent = this.LoadCommonBookingSection(bookingReqID, mailContent);

                        if (string.IsNullOrEmpty(mailContent))
                        {
                            throw (new Exception("Blank Template"));
                        }

                        foreach (DataRow dr in dt.Rows)
                        {
                            foreach (DataColumn dc in dt.Columns)
                            {

                                if (dc.DataType == System.Type.GetType("System.DateTime") || dc.DataType == System.Type.GetType("System.TimeSpan"))
                                {
                                    mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName.ToUpper()), DateFormatHelper.DisplayDate(DBHelper.GetDataColumnValue<DateTime>(dr, dc.ColumnName, DateTime.MinValue), "dd/MMM/yyyy"));
                                }
                                if (dc.ColumnName.ToUpper() == "HBL")
                                {
                                    hblList += (string.IsNullOrEmpty(hblList) ? "" : " / ") + DBHelper.GetDataColumnValue<string>(dr, dc.ColumnName, "");
                                }
                                else
                                {
                                    mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName.ToUpper()), DBHelper.GetDataColumnValue<string>(dr, dc.ColumnName, ""));
                                }
                            }
                            mbl = DBHelperBase.GetDataColumnValue<string>(dr, "MBL", "");
                        }

                    }

                    if (!string.IsNullOrEmpty(mbl))
                    {
                        sql = string.Format("select top 1 MBL_Remarks, HBL_Remarks, Remarks from PreAlert where MBL ='{0}' order by uID desc", DBHelperBase.ToDBStr(mbl));
                        dt = dbHelper.ExecDataTable(sql);
                        if (dt != null & dt.Rows.Count > 0)
                        {
                            mailContent = mailContent.Replace("<!--[[MBL_REMARK]]-->", DBHelperBase.GetDataColumnValue<string>(dt.Rows[0], "MBL_Remarks", ""));
                            mailContent = mailContent.Replace("<!--[[HBL_REMARK]]-->", DBHelperBase.GetDataColumnValue<string>(dt.Rows[0], "HBL_Remarks", ""));
                            mailContent = mailContent.Replace("<!--[[REMARKS]]-->", DBHelperBase.GetDataColumnValue<string>(dt.Rows[0], "Remarks", ""));
                        }
                    }
                }
                mailContent = mailContent.Replace("<!--[[HBLLIST]]-->", hblList);
                mailContent = mailContent.Replace("<!--[[NOW]]-->", DateFormatHelper.DisplayDateTime(DateTime.UtcNow, "dd/MMM/yyyy HH:mm:ss"));

                retVal = mailContent;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                retVal = string.Empty;
            }

            return retVal;
        }

        public string MBLFormContent(string bookingReqID)
        {
            string retVal = string.Empty;
            try
            {
                DataTable dt;
                DataRow dr;
                string sql = string.Empty;
                string mailContent = string.Empty;

                //Get mail template
                mailContent = this.LoadTemplate("MBLFORM");

                if (string.IsNullOrEmpty(mailContent))
                {
                    throw (new Exception("Blank Template"));
                }

                //load commmon section
                mailContent = this.LoadCommonBookingSection(bookingReqID, mailContent);

                //loading specify content
                sql = "select BookingReqID, ConsolidationID, CNEEStr as Consignee, ShipperStr as Shipper, NotifyPartyStr as NotifyParty, NotifyParty2Str as NotifyParty2, NotifyParty3Str as NotifyParty3, collect as Collect, mode as Mode, fax as Fax, notes as Notes, BLType from MBLForm ";
                sql += "where ISNULL(BookingReqID, '') = '{0}' ";
                sql = string.Format(sql, bookingReqID);

                using (DBHelper dbHelper = new DBHelper())
                {
                    dt = dbHelper.ExecDataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {

                            if (dc.DataType == System.Type.GetType("System.DateTime") || dc.DataType == System.Type.GetType("System.TimeSpan"))
                            {
                                mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName), DateFormatHelper.DisplayDate(DBHelper.GetDataColumnValue<DateTime>(dt.Rows[0], dc.ColumnName, DateTime.MinValue)));
                            }
                            else
                            {
                                mailContent = mailContent.Replace(string.Format("<!--[[{0}]]-->", dc.ColumnName), DBHelper.GetDataColumnValue<string>(dt.Rows[0], dc.ColumnName, ""));
                            }
                        }


                    }
                }

                mailContent = mailContent.Replace("<!--[[NOW]]-->", DateFormatHelper.DisplayDate(DateTime.Now));

                retVal = mailContent;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                retVal = string.Empty;
            }

            return retVal;
        }
    }
}
