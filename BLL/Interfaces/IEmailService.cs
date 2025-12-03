namespace BLL.Interfaces
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email, string token, string callbackUrl);
    }
}


