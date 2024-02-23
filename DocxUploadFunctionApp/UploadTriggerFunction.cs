using System.ComponentModel;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs.Models;
using MailKit.Net.Smtp;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MimeKit;
using static System.Reflection.Metadata.BlobBuilder;

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
            var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");



            try
            {
                var message = new MimeMessage();

                string subjectTo = "";

                message.From.Add(new MailboxAddress("Notification", "oleksii.chipizhenko@gmail.com"));

                message.To.Add(new MailboxAddress("Dear User", "mslvzn@gmail.com"));

                message.Subject = "Docx file";

                message.Body = new TextPart("plain")

                {
                    Text = "Yor docx already on blob storage!",
                };

                var client = new SmtpClient();

                client.Connect("smtp.gmail.com", 587, false);

                client.Authenticate("oleksii.chipizhenko@gmail.com", "katl xydv eoex eray");

                client.Send(message);

                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
            }
        }
    }
}
