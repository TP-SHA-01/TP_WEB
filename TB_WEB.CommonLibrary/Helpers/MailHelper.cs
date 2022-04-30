using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TB_WEB.CommonLibrary.Helpers
{
    public class MailHelper
    {
        private static String _mailAccount;
        private static String _password;
        private static String _smtpServer;
        private static int _port;
        public MailHelper(String mailAccount, String password, String smtpServer, int port = 587)
        {
            _mailAccount = mailAccount;
            _password = password;
            _smtpServer = smtpServer;
            _port = port;
        }
        public class FileAttachment
        {
            public String FileName { get; set; }
            public MemoryStream FileContent { get; set; }
        }
        public static bool SendMail(String toAddress, String subject, String text,bool IsBodyHtml,
            List<FileAttachment> fileAttachments = null)
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.To.Add(toAddress);
            /* 
            * msg.To.Add("b@b.com");
            * msg.To.Add("b@b.com");
            * msg.To.Add("b@b.com");
            */
            /* 
            * msg.CC.Add("c@c.com"); 
            * msg.CC.Add("c@c.com");
            */
            msg.From = new MailAddress(_mailAccount, _mailAccount, System.Text.Encoding.UTF8);
            /* The above three parameters are the sender's address (can write it casually), the sender's name, and the code.*/
            msg.Subject = subject;//mail title
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//mail header encoding
            msg.Body = text;//mail body
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = IsBodyHtml;
            msg.Priority = MailPriority.High;// Mail Priority
            if (fileAttachments != null && fileAttachments.Count != 0)
            {
                foreach (var fileAttachment in fileAttachments)
                {
                    msg.Attachments.Add(new Attachment(fileAttachment.FileContent, fileAttachment.FileName));
                }
            }
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(_mailAccount, _password);
            client.Port = _port;
            client.Host = _smtpServer;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
                return true;
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                return false;
            }
        }
    }
}
