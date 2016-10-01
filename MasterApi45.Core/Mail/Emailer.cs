using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MasterApi45.Core.Mail
{
    public interface IEmailer
    {
        void Send(string toEmail, string subject, string template, string cc = "", string bcc = "", string from = "", string fromName = "", object model = null);
        Task SendAsync(string toEmail, string subject, string template, string cc = "", string bcc = "", string from = "", string fromName = "", object model = null);
    }

    public class Emailer : IEmailer
    {
        #region Private Members
        private ITemplateRenderer _templateRenderer;
        #endregion

        #region Constructor
        public Emailer(ITemplateRenderer templateRenderer)
        {
            _templateRenderer = templateRenderer;
        }
        #endregion

        #region Public Methods
        public void Send(string toEmail, string subject, string template, string cc = "", string bcc = "", string from = "", string fromName = "", object model = null)
        {
            MailMessage message = ConfigureMailMessage(toEmail, subject, template, cc, bcc, from, fromName, model);
            SmtpClient client = new SmtpClient();

            if (string.IsNullOrEmpty(fromName))
            {
                message.From = new MailAddress(message.From.Address, ConfigurationManager.AppSettings["email:FromName"]);
            }
            else
            {
                message.From = new MailAddress(message.From.Address, fromName);
            }

            client.Send(message);
        }

        public async Task SendAsync(string toEmail, string subject, string template, string cc = "", string bcc = "", string from = "", string fromName = "", object model = null)
        {
            MailMessage message = ConfigureMailMessage(toEmail, subject, template, cc, bcc, from, fromName, model);

            if (string.IsNullOrEmpty(fromName))
            {
                message.From = new MailAddress(message.From.Address, ConfigurationManager.AppSettings["email:FromName"]);
            }
            else
            {
                message.From = new MailAddress(message.From.Address, fromName);
            }

            using (SmtpClient smtpClient = new SmtpClient())
            {
                await smtpClient.SendMailAsync(message);
            }
        }
        #endregion

        #region Private Methods
        private MailMessage ConfigureMailMessage<T>(string toEmail, string subject, string template, string cc = "", string bcc = "", string from = "", string fromName = "", T model = null) where T : class
        {
            MailMessage message = new MailMessage();
            if (!string.IsNullOrEmpty(from))
            {
                message.From = new MailAddress(from);
            }

            if (toEmail.Contains(";"))
            {
                string[] recipents = toEmail.Split(';');
                foreach (string recipent in recipents)
                {
                    message.To.Add(new MailAddress(recipent));
                }
            }
            else
            {
                message.To.Add(new MailAddress(toEmail));
            }

            message.Subject = _templateRenderer.Parse(subject, model);
            message.Body = _templateRenderer.Parse(template, model);

            if (!string.IsNullOrEmpty(cc))
            {
                message.CC.Add(cc);
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                message.Bcc.Add(bcc);
            }

            return message;
        }
        #endregion
    }
}
