using BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace BLL.Classes
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmailAsync(string email, string token, string callbackUrl)
        {
            var verificationUrl = $"{callbackUrl}?email={WebUtility.UrlEncode(email)}&token={WebUtility.UrlEncode(token)}";
            var emailBody = GetVerificationEmailTemplate(verificationUrl);

            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];

            if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(username) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(senderEmail))
            {
                // Log warning: Email settings not configured
                return;
            }

            var client = new SmtpClient(smtpServer)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, "FPTU Facility Booking System"),
                Subject = "Verify your email address",
                Body = emailBody,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }

        private string GetVerificationEmailTemplate(string verificationUrl)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email Verification - FPTU Facility Booking</title>
    <style>
        body {{
            margin: 0;
            padding: 0;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f5f5f5;
            color: #333;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
        }}
        .header h1 {{
            color: #0066cc;
            margin: 0;
        }}
        .content {{
            margin-bottom: 30px;
        }}
        .button {{
            display: inline-block;
            padding: 12px 30px;
            background-color: #0066cc;
            color: #ffffff;
            text-decoration: none;
            border-radius: 5px;
            margin: 20px 0;
        }}
        .footer {{
            text-align: center;
            color: #666;
            font-size: 12px;
            margin-top: 30px;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>FPTU Facility Booking System</h1>
        </div>
        <div class=""content"">
            <p>Thank you for registering with FPTU Facility Booking System.</p>
            <p>Please verify your email address by clicking the button below:</p>
            <a href=""{verificationUrl}"" class=""button"">Verify Email</a>
            <p>If the button doesn't work, copy and paste this link into your browser:</p>
            <p style=""word-break: break-all; color: #0066cc;"">{verificationUrl}</p>
            <p><small>This link will expire in 24 hours.</small></p>
        </div>
        <div class=""footer"">
            <p>&copy; 2025 FPTU Facility Booking System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}


