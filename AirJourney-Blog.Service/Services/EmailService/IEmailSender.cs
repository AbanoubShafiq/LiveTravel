using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.EmailService
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string toEmail, string Subject, string htmlBody);
    }
}
