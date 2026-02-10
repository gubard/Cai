namespace Cai.Models;

public sealed class FtpParameters
{
    public FtpParameters(string host, string login, string password)
    {
        Host = host;
        Login = login;
        Password = password;
    }

    public string Host { get; }
    public string Login { get; }
    public string Password { get; }
}
