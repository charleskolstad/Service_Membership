using ServiceMembership_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ServiceMembership_Core
{
    public class NotificationTools: INotificationTools
    {
        public void SendNotificationMail(string emailTo, string emailName, string emailBody)
        {
            try
            {
                MailAddress fromAddress = new MailAddress("charlespkolstad@gmail.com", "Charles");
                MailAddress toAddress = new MailAddress(emailTo, emailName);

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("charlespkolstad@gmail.com", "charles020810kolstad");
                MailMessage message = new MailMessage(fromAddress, toAddress);
                message.Subject = "New message from Service Membership.";
                message.Body = emailBody;

                smtp.Send(message);
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
            }
        }
    }

    public class FakeNotificationTools : INotificationTools
    {
        public void SendNotificationMail(string emailTo, string emailName, string emailBody)
        {
            
        }
    }

    public interface INotificationTools
    {
        void SendNotificationMail(string emailTo, string emailName, string emailBody);
    }
}
