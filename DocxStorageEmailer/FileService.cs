using Azure.Storage;
using Azure.Storage.Blobs;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DocxStorageEmailer
{
    public class FileService
    {
        private readonly string _storageAccount = "reenbitteststorage";
        private readonly string _key = "jse+7c8v/P9yeWdob2GgSTrJAUBcGqnpn6H1Y29mVyXSE8SQU3BG9oSQB8cJftzCQCQRNx+vMKVl+AStRoWE4Q==";
        private readonly BlobContainerClient _filesContainer;

        public FileService()
        {
            var credential = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _filesContainer = blobServiceClient.GetBlobContainerClient("files");
        }

        public virtual async Task<BlobResponseDto> UploadAsync(IFormFile blob, string email)
        {
            BlobResponseDto response = new();
            BlobClient client = _filesContainer.GetBlobClient(blob.FileName);

            await using (Stream? data = blob.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            IDictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("email", email.ToString());
            await client.SetMetadataAsync(metadata);

            response.Status = $"File {blob.FileName} Upload Done";
            response.Error = false;
            response.Blob.Url = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;
            

            return response;
        }
    }
}
