using API.Entities.OtherModel;
using API.Extensions;
using API.Helpers.Authorization;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("file")]
    public class FileController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        private static readonly Dictionary<string, string> ContentType = new()
        {
            {"jpg", "image/jpeg" },
            {"jpeg", "image/jpeg" },
            {"png", "image/png" },
            {"mp4", "video/mp4" },
        };

        private static readonly string UploadFolderPath = Path.Combine(Environment.CurrentDirectory, "UploadedFiles");
        private static readonly int DefaultBufferSize = 4096;

        public FileController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Role("Admin"), Role("Manager")]
        [HttpPost]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            ValidateFile(file);

            string name = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                        + "." + file.FileName.Split(".").Last();
            var filePath = Path.Combine(UploadFolderPath, name);

            if (!Directory.Exists(UploadFolderPath))
            {
                Directory.CreateDirectory(UploadFolderPath);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, DefaultBufferSize))
            {
                await file.CopyToAsync(fileStream);
            }

            var uploadedFile = new UploadedFile()
                {
                    DateCreated = DateTime.UtcNow,
                    ContentType = file.ContentType,
                    Name = name,
                    Extension =  file.FileName.Split(".").Last(),
                    Path = filePath,
                    CreatedByUserId = User.GetUserId()
                };

            _unitOfWork.FileRepository.Insert(uploadedFile);

            if(await _unitOfWork.Complete())
                return Ok(uploadedFile.Id);
        
            return BadRequest("Can not upload file");
        }

        [AllowAnonymous]
        [HttpGet("{name}")]
        public async Task<ActionResult> DownloadFile(string name)
        {
            var file = await GetUploadedFileInformation(name);

            byte[] content;
            using (var fileStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.None, DefaultBufferSize))
            {
                content = new byte[fileStream.Length];
                await fileStream.ReadAsync(content.AsMemory(0, (int)fileStream.Length));
            }

            return File(content, file.ContentType, file.Name);

        }


        public async Task<UploadedFile> GetUploadedFileInformation(int id)
        {
            var uploadedFile = await _unitOfWork.FileRepository.GetById(id);
            if (uploadedFile == null)
                throw new KeyNotFoundException("File not found");
            return uploadedFile;
        }

        public async Task<UploadedFile> GetUploadedFileInformation(string name)
        {
            var uploadedFile = await _unitOfWork.FileRepository.GetFirstBy(x => x.Name == name);
            if (uploadedFile == null)
                throw new KeyNotFoundException("File not found");
            return uploadedFile;
        }
        
        private static void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                throw new ValidationException("File is empty");

            if (!ContentType.Any(ct => ct.Value == file.ContentType))
                throw new ValidationException("Wrong file type");
        }
    }
}
