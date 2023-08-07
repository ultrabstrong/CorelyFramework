using BusinessLayer.Core;
using Corely.Helpers;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        /// <summary>
        /// Desktop path
        /// </summary>
        static readonly string DesktopLoc = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";
        static readonly string DownloadsLoc = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\";

        /// <summary>
        /// Entry point for console test
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                TestAirTable().GetAwaiter();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught:{Environment.NewLine}{ex}");
            }
            Console.WriteLine("Program finished. Press any key to exit");
            Console.ReadKey();
        }

        public class Application
        {
            public string name { get; set; }
            public string notes { get; set; }

            public List<AirtableApiClient.AirtableAttachment> applicationpdf { get; set; }
        }

        static async Task TestAirTable()
        {
            AirtableApiClient.AirtableBase baseClient = new AirtableApiClient.AirtableBase("key1", "key2");

            AirtableApiClient.Fields fields = new AirtableApiClient.Fields();

            fields.AddField("name", "appTest");


            string apptext = File.ReadAllText(DownloadsLoc + "app.pdf");
            apptext = Corely.Data.Encoding.Base64String.Base64Encode(apptext);
            fields.AddField("notes", apptext);

            var res = await baseClient.CreateRecord("applications", fields);

            var record = await baseClient.RetrieveRecord<Application>("applications", "appTest");
            apptext = Corely.Data.Encoding.Base64String.Base64Decode(record.Record.Fields.notes);
            File.WriteAllText(apptext, DownloadsLoc + "app2.pdf");

            /*
            var records = await baseClient.ListRecords<Application>("applications");
            foreach (var r in records.Records)
            {
                Console.WriteLine($"{r.Fields.name} - {r.Fields.notes}");
            }
            */
        }

        static void TestEmailRegex()
        {
            var reg = new Regex(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$");
            var email = "ultrabstrong@gmail.com";
            Console.WriteLine(reg.IsMatch(email));
        }

        /// <summary>
        /// Test converting html to pdf
        /// </summary>
        static void TestHTMLtoPDF()
        {
            string filepath = $"{DesktopLoc}sample.html";
            string outpath = DesktopLoc + FilePathHelper.GetFileNameNoExt(filepath) + ".pdf";
            string html = File.ReadAllText(filepath);
            // Convert with SelectPDF
            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;
            converter.Options.PdfDocumentInformation.Author = "ApexProperties";
            converter.Options.PdfDocumentInformation.CreationDate = DateTime.Now;
            converter.Options.PdfDocumentInformation.Title = "Application";
            converter.Options.PdfDocumentInformation.Subject = "Subect";
            PdfDocument doc = converter.ConvertHtmlString(html);
            byte[] pdfBytes = doc.Save();
            doc.Close();
            // close pdf document
            if (File.Exists(outpath)) { File.Delete(outpath); }
            using (FileStream fs = new FileStream(outpath, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(pdfBytes, 0, pdfBytes.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// Test sending mail
        /// </summary>
        static void TestMail()
        {
            MailSettings settings = new MailSettings()
            {
                SMTPServer = "server",
                SMTPUsername = "username",
                SMTPPw = "pw",
                SMTPPort = 25,
                SMTPTo = "to"
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
                    Subject = $"test subject",
                    From = new MailAddress(settings.SMTPUsername)
                };

                // Set body to html content
                message.Body = $"test body";
                message.IsBodyHtml = false;

                // Add recipients
                message.To.Add(settings.SMTPTo);

                // Send message
                client.Send(message);
            }
            // Dispose client
            finally { if (client != null) client.Dispose(); }
        }
    }
}
