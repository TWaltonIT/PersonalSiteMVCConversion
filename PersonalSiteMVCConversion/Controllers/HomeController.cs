using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using PersonalSiteMVCConversion.Models;
using System.Diagnostics;
using MimeKit;
using MailKit.Net.Smtp;
using PaulMiami.AspNetCore.Mvc.Recaptcha;

namespace PersonalSiteMVCConversion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CredentialSettings _credentials;

        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IOptions<CredentialSettings> settings, IConfiguration config)
        {
            _logger = logger;
            _credentials = settings.Value;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Resume()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();

            //We want the info from our Contact form to use the ContactViewModel we created.
            //To do this, we cna generate the necessary code using the following steps:

     
        }

        //Now we need to handle what to do when the user submits the form. For this,
        //we will make another Contact Action, this time intended to handle the POST request
        [HttpPost]
        public IActionResult Contact(ContactViewModel cvm)
        {
            ViewBag.RecaptchaError = null;

            if (!ModelState.IsValid)
            {
                ViewBag.RecaptchaError = "Please confirm you are not a robot before sending a message.";

                return View(cvm);
            }


            //Create the format for the message content we will receive from the contact form
            string message = $"You have received a new email from your site's contact form! <br />" +
                $"Sender: {cvm.Name}<br />Email: {cvm.Email}<br />Subject: {cvm.Subject}<br />" +
                $"Message: {cvm.Message}";

            var mm = new MimeMessage();

            //We can access the credentials for this email user from our appsettings.json file as shown below:
            mm.From.Add(new MailboxAddress("Sender", _credentials.Email.Username));

            //The recipient of this email will be our personal email address, also stored in appsettings.json
            mm.To.Add(new MailboxAddress("Personal", _credentials.Email.Recipient));

            //The subject will be the one provided by the user, which we stored in our cvm object
            mm.Subject = $"New contact form message: [{cvm.Subject}]";

            //The body of the message will be formatted with the string we created above
            mm.Body = new TextPart("HTML") { Text = message };

            //We can set priority of the message as "urgent" so it will be flagged in our email client.
            mm.Priority = MessagePriority.Urgent;

            //We can also add the user's provided email address to the list of ReplyTo addresses
            //so our replies can be sent directly to them instead of the email user on our hosting provider.
            mm.ReplyTo.Add(new MailboxAddress("User", cvm.Email));

            //The using directive will create the SmtpClient object used to send the email.
            //Once all of the code inside of the using directive's scope has been executed,
            //it will close any open connections and dispose of the object for us.
            using (var client = new SmtpClient())
            {
                //Connect to the mail server using credentials in our appsettings.json & port 8889
                client.Connect(_credentials.Email.Server, 8889);

                //Log in to the mail server
                client.Authenticate(_credentials.Email.Username, _credentials.Email.Password);

                //It's possible the mail server may be down when the user attempts to contact us,
                //so we can "encapsulate" our code to send the message in a try/catch
                try
                {
                    //Try to send the email:
                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    //If there is an issue, we can store an error message in a ViewBag variable
                    //to be displayed in the View.
                    ViewBag.ErrorMessage = $"There was an error in processing your request. " +
                        $"Please try again later.<br />Error Message: {ex.StackTrace}";

                    //Return the user to the View with their form information intact
                    return View(cvm);
                }
            }

            //If all goes well, return a View that displays a confirmation to the user
            //that their email was sent.
            return View("EmailConfirmation", cvm);
        }

    }
}
