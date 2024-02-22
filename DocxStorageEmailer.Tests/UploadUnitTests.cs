using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using DocxStorageEmailer;
using DocxStorageEmailer.Controllers;
using Azure;
using System.Reflection.Metadata;
using static System.Reflection.Metadata.BlobBuilder;

namespace DocxStorageEmailer.Tests
{
    public class ValueControllerTests
    {
        private readonly ValuesController _controller;
        private readonly Mock<FileService> _mockFileService;

        public ValueControllerTests()
        {
            _mockFileService = new Mock<FileService>();
            _controller = new ValuesController(_mockFileService.Object);
        }

        [Fact]
        public async Task Upload_ValidFile_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.FileName).Returns("test.docx");
            fileMock.Setup(_ => _.OpenReadStream()).Returns(new MemoryStream());

            _mockFileService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync(new BlobResponseDto());

            // Act
            var result = await _controller.Upload(fileMock.Object) as OkObjectResult;

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task Upload_InvalidFile_ReturnsBadRequest()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.FileName).Returns("test.txt"); // Not a docx file

            // Act
            var result = await _controller.Upload(fileMock.Object) as BadRequestObjectResult;

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(400, result.StatusCode);
        }
    }
}