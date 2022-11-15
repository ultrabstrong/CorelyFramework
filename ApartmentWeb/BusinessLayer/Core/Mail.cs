using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLayer.Core
{
    public static class Mail
    {
        /// <summary>
        /// Send application email
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="application"></param>
        /// <param name="applicationhtml"></param>
        /// <param name="applicationpdf"></param>
        public static void SendApplication(MailSettings settings, Application application, string applicationhtml, Stream applicationpdf)
        {
            // Create PDF attachment
            List<Attachment> attachments = new List<Attachment>()
            {
                new Attachment(applicationpdf, $"{application.PersonalInfo.FirstName} {application.PersonalInfo.LastName} {application}.pdf")
            };

            // Initialize SMTP client
            SmtpClient client = null;
            try
            {
                // Build client
                client = new SmtpClient(settings.SMTPServer);
                client.Port = 25;
                client.Credentials = new NetworkCredential(settings.SMTPUsername, settings.SMTPPw);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                // Create message
                MailMessage message = new MailMessage()
                {
                    Subject = $"Application for {application.RentalAddress} from {application.PersonalInfo.FirstName} {application.PersonalInfo.LastName}; Co-Applicants : {application.OtherApplicants}",
                    From = new MailAddress(settings.SMTPUsername)
                };

                // Add attachments
                foreach (Attachment a in attachments) { message.Attachments.Add(a); }

                // Set body to html content
                /* Uncomment to set message body to html used to generate attachment
                message.Body = applicationhtml;
                message.IsBodyHtml = true;
                */
                message.Body = $"Attached is the application for {application.RentalAddress} from {application.PersonalInfo.FirstName} {application.PersonalInfo.LastName}";
                message.IsBodyHtml = false;

                // Add recipient
#if DEBUG
                message.To.Add("ultrabstrong@gmail.com");
#else
                message.To.Add(settings.SMTPTo);
#endif
                Regex emailReg = new Regex(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$");
                if (emailReg.IsMatch(application.PersonalInfo.Email))
                {
                    message.ReplyToList.Clear();
                    message.ReplyToList.Add(application.PersonalInfo.Email);
                }

                // Send message
                client.Send(message);
            }
            // Dispose client
            finally { if (client != null) client.Dispose(); }
        }

        /// <summary>
        /// Send maintenance request email
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="maintenanceRequest"></param>
        /// <param name="applicationhtml"></param>
        /// <param name="applicationpdf"></param>
        public static void SendMaintenanceRequest(MailSettings settings, MaintenanceRequest maintenanceRequest, string applicationhtml, Stream applicationpdf)
        {
            // Create PDF attachment
            List<Attachment> attachments = new List<Attachment>()
            {
                new Attachment(applicationpdf, $"{maintenanceRequest.FirstName} {maintenanceRequest.LastName} Maintenance Request.pdf")
            };

            // Initialize SMTP client
            SmtpClient client = null;
            try
            {
                // Build client
                client = new SmtpClient(settings.SMTPServer);
                client.Port = 25;
                client.Credentials = new NetworkCredential(settings.SMTPUsername, settings.SMTPPw);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                // Create message
                MailMessage message = new MailMessage()
                {
                    Subject = $"Maintenance request for {maintenanceRequest.RentalAddress} from {maintenanceRequest.FirstName} {maintenanceRequest.LastName}",
                    From = new MailAddress(settings.SMTPUsername)
                };

                // Add attachments
                foreach (Attachment a in attachments) { message.Attachments.Add(a); }

                // Set body to html content
                /* Uncomment to set message body to html used to generate attachment
                message.Body = applicationhtml;
                message.IsBodyHtml = true;
                */
                message.Body = $@"Attached is the maintenance request from {maintenanceRequest.FirstName} {maintenanceRequest.LastName} for {maintenanceRequest.RentalAddress}
Email: {(string.IsNullOrWhiteSpace(maintenanceRequest.Email) ? "Not provided" : maintenanceRequest.Email)}
Phone: {(string.IsNullOrWhiteSpace(maintenanceRequest.Phone) ? "Not provided" : maintenanceRequest.Phone)}

{maintenanceRequest.Description}";
                message.IsBodyHtml = false;

                // Add recipients
#if DEBUG
                message.To.Add("ultrabstrong@gmail.com");
#else
                message.To.Add(settings.SMTPTo);
#endif
                Regex emailReg = new Regex(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$");
                if (emailReg.IsMatch(maintenanceRequest.Email))
                {
                    message.ReplyToList.Clear();
                    message.ReplyToList.Add(maintenanceRequest.Email);
                }

                // Send message
                client.Send(message);
            }
            // Dispose client
            finally { if (client != null) client.Dispose(); }
        }
    }
}
