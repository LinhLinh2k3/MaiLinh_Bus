using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace NhaXeMaiLinh.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly MailSetting _mailSetting;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<MailSetting> mailSettings, ILogger<EmailSender> logger)
        {
            _mailSetting = mailSettings.Value;
            _logger = logger;
            _logger.LogInformation("Created MailSender service!");
        }

        private async void Send(MimeMessage mail)
        {
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                smtp.Connect(_mailSetting.Host, _mailSetting.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSetting.Mail, _mailSetting.Password);
                await smtp.SendAsync(mail);
            }
            catch (Exception ex)
            {
                // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                Directory.CreateDirectory("mailssave");
                var draftMailFile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await mail.WriteToAsync(draftMailFile);

                _logger.LogInformation("Lỗi gửi mail, lưu tại - " + draftMailFile);
                _logger.LogError(ex.Message);
            }
            smtp.Disconnect(true);
        }

        // Gửi mail cho một người (không có tệp đính kèm)
        public Task SendEmail(string email, string subject, string htmlMessage)
        {
            var mail = new MimeMessage();
            mail.Sender = new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail);
            mail.From.Add(new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail));
            mail.To.Add(MailboxAddress.Parse(email));
            mail.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            mail.Body = builder.ToMessageBody();

            Send(mail);
            return Task.CompletedTask;
        }

        // Gửi mail cho một người (có 1 tệp đính kèm)
        public async Task SendEmail(string email, string subject, string htmlMessage, IFormFile attachment)
        {
            var mail = new MimeMessage();
            mail.Sender = new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail);
            mail.From.Add(new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail));
            mail.To.Add(MailboxAddress.Parse(email));
            mail.Subject = subject;

            var multipart = new Multipart("mixed");
            var builder = new BodyBuilder { HtmlBody = htmlMessage };
            multipart.Add(builder.ToMessageBody());
            using (var memoryStream = new MemoryStream())
            {
                await attachment.CopyToAsync(memoryStream);
                var attachmentPart = new MimePart(attachment.ContentType)
                {
                    Content = new MimeContent(memoryStream, ContentEncoding.Default),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(attachment.FileName)
                };
                multipart.Add(attachmentPart);
            }
            mail.Body = multipart;

            Send(mail);
        }

        // Gửi mail cho một người (có nhiều tệp đính kèm)
        public async Task SendEmail(string email, string subject, string htmlMessage, List<IFormFile> attachments)
        {
            var mail = new MimeMessage();
            mail.Sender = new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail);
            mail.From.Add(new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail));
            mail.To.Add(MailboxAddress.Parse(email));
            mail.Subject = subject;

            var multipart = new Multipart("mixed");
            var builder = new BodyBuilder { HtmlBody = htmlMessage };
            multipart.Add(builder.ToMessageBody());
            foreach (var attachment in attachments)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await attachment.CopyToAsync(memoryStream);
                    var attachmentPart = new MimePart(attachment.ContentType)
                    {
                        Content = new MimeContent(memoryStream, ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName(attachment.FileName)
                    };
                    multipart.Add(attachmentPart);
                }
            }
            mail.Body = multipart;

            Send(mail);
        }

        // Gửi mail cho nhiều người (không có tệp đính kèm)
        public Task SendMassEmail(List<string> emails, string subject, string htmlMessage)
        {
            var mail = new MimeMessage();
            mail.Sender = new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail);
            mail.From.Add(new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail));
            foreach (var receiver in emails)
            {
                mail.To.Add(MailboxAddress.Parse(receiver));
            }
            mail.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            mail.Body = builder.ToMessageBody();

            Send(mail);
            return Task.CompletedTask;
        }

        // Gửi mail cho nhiều người (có 1 tệp đính kèm)
        public async Task SendMassEmail(List<string> emails, string subject, string htmlMessage, IFormFile attachment)
        {
            var mail = new MimeMessage();
            mail.Sender = new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail);
            mail.From.Add(new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail));
            foreach (var receiver in emails)
            {
                mail.To.Add(MailboxAddress.Parse(receiver));
            }
            mail.Subject = subject;

            var multipart = new Multipart("mixed");
            var builder = new BodyBuilder { HtmlBody = htmlMessage };
            multipart.Add(builder.ToMessageBody());
            using (var memoryStream = new MemoryStream())
            {
                await attachment.CopyToAsync(memoryStream);
                var attachmentPart = new MimePart(attachment.ContentType)
                {
                    Content = new MimeContent(memoryStream, ContentEncoding.Default),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(attachment.FileName)
                };
                multipart.Add(attachmentPart);
            }
            mail.Body = multipart;

            Send(mail);
        }

        // Gửi mail cho nhiều người (có nhiều tệp đính kèm)
        public async Task SendMassEmail(List<string> emails, string subject, string htmlMessage, List<IFormFile> attachments)
        {
            var mail = new MimeMessage();
            mail.Sender = new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail);
            mail.From.Add(new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail));
            foreach (var receiver in emails)
            {
                mail.To.Add(MailboxAddress.Parse(receiver));
            }
            mail.Subject = subject;

            var multipart = new Multipart("mixed");
            var builder = new BodyBuilder { HtmlBody = htmlMessage };
            multipart.Add(builder.ToMessageBody());
            foreach (var attachment in attachments)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await attachment.CopyToAsync(memoryStream);
                    var attachmentPart = new MimePart(attachment.ContentType)
                    {
                        Content = new MimeContent(memoryStream, ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName(attachment.FileName)
                    };
                    multipart.Add(attachmentPart);
                }
            }
            mail.Body = multipart;

            Send(mail);
        }










    }
}