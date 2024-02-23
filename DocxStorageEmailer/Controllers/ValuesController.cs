using Azure.Storage.Blobs;
using Azure;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System;
using System.IO;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocxStorageEmailer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly FileService _fileService;

        public ValuesController(FileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string email)
        {
            string formatChecker = System.IO.Path.GetExtension(file.FileName);
            if (file != null && formatChecker == ".docx")
            {
                var result = await _fileService.UploadAsync(file, email);
                return Ok(result);
            }
            else
            {
                return BadRequest("Not a docx file!");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            return Ok();
        }
    }
}
