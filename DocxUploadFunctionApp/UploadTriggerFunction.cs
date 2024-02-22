using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace DocxUploadFunctionApp
{
    public class UploadTriggerFunction
    {
        private readonly ILogger<UploadTriggerFunction> _logger;

        public UploadTriggerFunction(ILogger<UploadTriggerFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(UploadTriggerFunction))]
        public async Task Run([BlobTrigger("%ContainerName%", Connection = "AzureWebJobsStorage")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Test Project", "oleksii.chipizhenko@gmail.com"));

            message.To.Add(new MailboxAddress("Oleksii", "mslvzn@gmail.com"));

            message.Subject = "Your email uploaded";

            message.Body = new TextPart("plain")

            {
                Text = "Test message from blolb",
            };

            var client = new SmtpClient();

            client.Connect("smtp.gmail.com", 587, false);

            client.Authenticate("oleksii.chipizhenko@gmail.com", "katl xydv eoex eray");

            client.Send(message);

            client.Disconnect(true);
        }
    }
}
