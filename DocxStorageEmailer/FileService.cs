using Azure.Storage;
using Azure.Storage.Blobs;

namespace DocxStorageEmailer
{
    public class FileService
    {
        private readonly string _storageAccount = "reenbitteststorage";
        private readonly string _key = "aVqk69tuAv37G4YtG16C/zDGmYhchfAE4msgZknoC9sYcukHnL8S0f2DXkxPYc5EU6q5vFPx5uEA+AStPLfiYA==";
        private readonly BlobContainerClient _filesContainer;

        public FileService()
        {
            var credential = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _filesContainer = blobServiceClient.GetBlobContainerClient("files");
        }


        public virtual async Task<BlobResponseDto> UploadAsync(IFormFile blob)
        {
            BlobResponseDto response = new();
            BlobClient client = _filesContainer.GetBlobClient(blob.FileName);

            await using (Stream? data = blob.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            response.Status = $"File {blob.FileName} Upload Done";
            response.Error = false;
            response.Blob.Url = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;

            return response;
        }
    }
}
