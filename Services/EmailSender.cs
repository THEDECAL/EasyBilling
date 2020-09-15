using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EasyBilling.Services
{
    public class EmailSender : IEmailSender
    {
        const string CFG_FILE_NAME = "Settings\\emailServer.json";
        private readonly string _senderMailAddress = "";
        private readonly string _senderMailPassword = "";
        private readonly string _smtpServerAddress = "";
        private readonly int _smtpServerPort;
        private readonly SmtpClient _smtpClient;

        public EmailSender()
        {
            try
            {
                var config = new ConfigurationBuilder().AddJsonFile(CFG_FILE_NAME).Build();
                _senderMailAddress = config["login"];
                _senderMailPassword = config["password"];
                _smtpServerAddress = config["smtp-server"];
                _smtpServerPort = int.Parse(config["smtp-port"]);

                _smtpClient = new SmtpClient(_smtpServerAddress, _smtpServerPort);
                _smtpClient.Credentials = new NetworkCredential(_senderMailAddress, _senderMailPassword);
                _smtpClient.EnableSsl = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error. Can not read properties '{CFG_FILE_NAME}' in file.");
                Console.WriteLine(ex.Message);
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (email != null && subject != null && htmlMessage != null)
            {
                var srcMailAddress = new MailAddress(_senderMailAddress);
                var dstMailAddress = new MailAddress(email);
                var msg = new MailMessage(srcMailAddress, dstMailAddress);

                msg.Subject = subject;
                msg.Body = htmlMessage;
                msg.IsBodyHtml = true;

                if (_smtpClient != null) {
                    try
                    {
                        await Task.Run(() => _smtpClient.Send(msg));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unsuccessful attempt to send a mail ({(!string.IsNullOrWhiteSpace(email)?email:"email address is empty")}).");
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            else throw new NullReferenceException();
        }
    }
}
