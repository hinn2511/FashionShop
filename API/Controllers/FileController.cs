using API.Data;
using API.DTOs.Response.FileResponse;
using API.Entities.OtherModel;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "ManagerOnly")]
    [ApiController]
    [Route("file")]
    public class FileController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public FileController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            FileExtensions.ValidateFile(file, Constant.UploadContentType, 20000);

            var filePath = await FileExtensions.SaveFile(file);

            var fileName = filePath.Split("\\").Last();

            var fileExtension = filePath.Split(".").Last();

            var uploadedFile = new UploadedFile()
            {
                ContentType = file.ContentType,
                Name = fileName,
                Extension = fileExtension,
                Path = filePath,
            };

            uploadedFile.AddCreateInformation(GetUserId());

            _unitOfWork.FileRepository.Insert(uploadedFile);

            if (await _unitOfWork.Complete())
                return Ok($"{Constant.DownloadFileUrl}{uploadedFile.Name}");

            return BadRequest("Can not upload file");
        }

        [HttpPost("image")]
        public async Task<ActionResult> UploadImageFile(IFormFile file, [FromQuery] int height, [FromQuery] int width, [FromQuery] bool keepRatio)
        {
            if (!FileExtensions.ValidateFile(file, Constant.ImageContentType, 10000))
                return BadRequest("File not valid");

            var filePath = await FileExtensions.SaveFile(file);

            var uploadedFile = new UploadedFile();

            if (height != 0 && width != 0)
            {
                var keepSourceImage = file.ContentType == "image/png";

                var resizedFilePath = FileExtensions.ResizeImage(width, height, filePath, false, keepRatio, keepSourceImage);

                var resizedFileName = resizedFilePath.Split("\\").Last();

                var resizedFileExtension = resizedFileName.Split(".").Last();

                uploadedFile.ContentType = file.ContentType;
                uploadedFile.Name = resizedFileName;
                uploadedFile.Extension = resizedFileExtension;
                uploadedFile.Path = resizedFilePath;
            }
            else
            {
                uploadedFile.ContentType = file.ContentType;
                uploadedFile.Name = filePath.Split("\\").Last();
                uploadedFile.Extension = filePath.Split(".").Last();
                uploadedFile.Path = filePath;
            }
        
            uploadedFile.AddCreateInformation(GetUserId());
            _unitOfWork.FileRepository.Insert(uploadedFile);

            if (await _unitOfWork.Complete())
                return Ok(new FileUploadedResponse($"{Constant.DownloadFileUrl}{uploadedFile.Name}"));

            return BadRequest("Can not upload file");
        }

        [AllowAnonymous]
        [HttpGet("{name}")]
        public async Task<ActionResult> DownloadFile(string name)
        {
            var file = await GetUploadedFileInformation(name);

            byte[] content;
            ////  download only and not viewable in browser
            //  using (var fileStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.None, Constant.DefaultBufferSize))
            //     {
            //         content = new byte[fileStream.Length];
            //         await fileStream.ReadAsync(content.AsMemory(0, (int)fileStream.Length));
            //     }
            //     return File(content, file.ContentType, file.Name);

            //// download from bytes and viewable in browser
            using (var fileStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.None, Constant.DefaultBufferSize))
            {
                content = new byte[fileStream.Length];
                await fileStream.ReadAsync(content.AsMemory(0, (int)fileStream.Length));
            }

            ////  for video file
            if (file.Extension == ".mp4")
                return File(content, file.ContentType, file.Name);

            return new FileContentResult(content, file.ContentType);

            //// download from stream and viewable in browser
            // var fileStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read);
            // return new FileStreamResult(fileStream, file.ContentType);

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

    }
}

