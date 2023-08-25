namespace Identity.Models;

public class AppSettings
{


    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SigningKey { get; set; }

}