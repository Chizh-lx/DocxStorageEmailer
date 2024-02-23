using System;
using System.ComponentModel;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using MailKit.Net.Smtp;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
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
        public async Task Run([BlobTrigger("%ContainerName%", Connection = "AzureWebJobsStorage")] Stream stream, string name, IDictionary<string, string> metaData)
        {
            var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");

            try
            {
                var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
                var blobContainerClient = blobServiceClient.GetBlobContainerClient("files");
                var blobClient = blobContainerClient.GetBlobClient(name);

                var sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = "files",
                    BlobName = name,
                    Resource = "b",
                    StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
                    Protocol = SasProtocol.Https
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sasQueryParameters = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(blobServiceClient.AccountName, Environment.GetEnvironmentVariable("AzureWebJobsStorageKey")));
                var blobUriWithSas = blobClient.Uri + "?" + sasQueryParameters;

                ///------------

                var message = new MimeMessage();

                string subjectTo = metaData.Values.First();

                message.From.Add(new MailboxAddress("Notification", "oleksii.chipizhenko@gmail.com"));

                message.To.Add(new MailboxAddress("Dear User", subjectTo));

                message.Subject = "Docx file";

                message.Body = new TextPart("plain")

                {
                    Text = blobUriWithSas,
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
