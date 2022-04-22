using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace IberaDelivery.Services;

public class EmailSender
{
    public void SendActivationEmail(string userEmail, string token){
        MailAddress to = new MailAddress(userEmail);
        MailAddress from = new MailAddress("iberiadelivery@gmail.com");

        MailMessage message = new MailMessage(from, to);
        message.Subject = "Activation Account";
        message.IsBodyHtml = true;

        string htmlString = @"<html>
                      <body>
                      <p>An account has been created with the email "+ userEmail +@", associated please click the link to activate the account.</p>
                      <p><a href='https://localhost:7274/User/ActivateAccount?token="+ token +@"'>Link</p>
                      </body>
                      </html>
                     "; 
        
        message.Body = htmlString;

        SmtpClient client = new SmtpClient("smtp.server.address", 2525)
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new NetworkCredential(from.Address, "IberiaDeliveryP@ssw0rd"),
            Timeout = 20000
        };
        // code in brackets above needed if authentication required

        try
        {
        client.Send(message);
        }
        catch (SmtpException ex)
        {
        Console.WriteLine(ex.ToString());
        }
    }
}