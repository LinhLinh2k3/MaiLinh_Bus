namespace NhaXeMaiLinh.Services.Email
{
    public interface IEmailSender
    {
        Task SendEmail(string email, string subject, string htmlMessage);
        Task SendEmail(string email, string subject, string htmlMessage, IFormFile attachment);
        Task SendEmail(string email, string subject, string htmlMessage, List<IFormFile> attachments);
        Task SendMassEmail(List<string> emails, string subject, string htmlMessage);
        Task SendMassEmail(List<string> emails, string subject, string htmlMessage, IFormFile attachment);
        Task SendMassEmail(List<string> emails, string subject, string htmlMessage, List<IFormFile> attachments);
    }
}