namespace DocxStorageEmailer
{
    public class BlobResponseDto
    {
        public BlobResponseDto()
        {
            Blob = new BloblDto();
        }

        public string? Status { get; set; }
        public bool Error { get; set; }

        public BloblDto Blob { get; set; }
    }
}
