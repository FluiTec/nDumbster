using System;
using System.Linq;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using nDumbsterCore.smtp;

namespace nDumbsterCoreSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = SimpleSmtpServer.Start(25);

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true; // accepts all certs
                client.Connect("127.0.0.1", 25, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Send(new MimeMessage
                {
                    From = { new MailboxAddress("Test", "test@example.com") },
                    To = { new MailboxAddress("test@example.com", "Test") },
                    Subject = "Subject",
                    Body = new TextPart(TextFormat.Plain)
                });
            }
            
            Console.WriteLine($"Received mails on server: {server.ReceivedEmail.Count()}");
            Console.WriteLine("Press <Enter> to quit.");
            Console.ReadLine();

            server.Stop();
        }
    }
}
