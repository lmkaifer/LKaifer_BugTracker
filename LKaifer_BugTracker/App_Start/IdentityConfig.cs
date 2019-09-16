using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using LKaifer_BugTracker.Models;
using System.Web.Configuration;
using System.Net.Mail;
using System.Net;
using FluentEmail.Mailgun;
using FluentEmail.Core;
using RestSharp;
using RestSharp.Authenticators;
using Google.Apis.Gmail.v1;

namespace LKaifer_BugTracker
{ 
    public class PersonalEmail
{
    public async Task SendAsync(MailMessage message)
    {
            //var mailgunApiKey = WebConfigurationManager.AppSettings["mailgunapikey"];
            //var mailgundomain = WebConfigurationManager.AppSettings["mailgundomain"];

            //            var sender = new MailgunSender(
            //    mailgundomain, // Mailgun Domain
            //    mailgunApiKey // Mailgun API Key
            //);
            //            Email.DefaultSender = sender;
            //            var email = Email
            //    .From(message.From.Address)
            //    .To(message.To.FirstOrDefault().Address)
            //    .Subject(message.Subject)
            //    .Body(message.Body);

            //            try
            //            {
            //                var response = await email.SendAsync();
            //            }
            //            catch(Exception ex)
            //            {
            //                Console.WriteLine(ex.Message);
            //            }


            //RestClient client = new RestClient();
            //client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            //client.Authenticator =
            //    new HttpBasicAuthenticator("api",
            //                                mailgunApiKey);
            //RestRequest request = new RestRequest();
            //request.AddParameter("domain", mailgundomain, ParameterType.UrlSegment);
            //request.Resource = "{domain}/messages";
            //request.AddParameter("from", "Excited User <lmkaifer@hotmail.com>");
            //request.AddParameter("to", message.To.FirstOrDefault().Address);
            //request.AddParameter("to", message.To.FirstOrDefault().Address);
            //request.AddParameter("subject", "Hello");
            //request.AddParameter("text", "Testing some Mailgun awesomness!");
            //request.Method = Method.POST;
            //var response = client.Execute(request);

            var GmailUsername = WebConfigurationManager.AppSettings["username"];
            var GmailPassword = WebConfigurationManager.AppSettings["password"];
            var host = WebConfigurationManager.AppSettings["host"];
            int port = Convert.ToInt32(WebConfigurationManager.AppSettings["port"]);

            using (var smtp = new SmtpClient()
            {
                Host = host,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(GmailUsername, GmailPassword)
            })
            {
                try
                {
                    await smtp.SendMailAsync(message);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await Task.FromResult(0);
                }
            };

        }
}

    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            var GmailUsername = WebConfigurationManager.AppSettings["username"];
            MailMessage emailMsg = new MailMessage(GmailUsername, message.Destination)
            {
                Body = message.Body,
                Subject = message.Subject,
                IsBodyHtml = true
            };
            PersonalEmail email = new PersonalEmail();
            await email.SendAsync(emailMsg);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
