﻿using API.Data;
using API.DTOs.Params;
using API.DTOs.Response;
using API.DTOs.Response.FileResponse;
using API.DTOs.Response.PhotoResponse;
using API.Entities.Other;
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
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class FileController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;

        public FileController(IMapper mapper, IUnitOfWork unitOfWork, IPhotoService photoService)
        {
            _photoService = photoService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize(Roles = "UploadFile")]
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
                return Ok($"{Constant.DownloadapiUrl}{uploadedFile.Name}");

            return BadRequest("Can not upload file");
        }

        [Authorize(Roles = "UploadImage")]
        [HttpPost("image")]
        public async Task<ActionResult> UploadImageFile(IFormFile file, [FromQuery] int height, [FromQuery] int width, [FromQuery] double ratio, [FromQuery] string cropOption)
        {
            if (!FileExtensions.ValidateFile(file, Constant.ImageContentType, 10000))
                return BadRequest("File not valid");

            var result = await _photoService.AddPhotoAsync(file, height, width, cropOption, ratio);

            if (result.Error != null) 
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "Can not upload photo"));
            
            var image = new Photo();

            image.PublicId = result.PublicId;
            image.Url = result.SecureUrl.AbsoluteUri;

            _unitOfWork.PhotoRepository.Insert(image);

            if (await _unitOfWork.Complete())
                return Ok(new FileUploadedResponse(image.Id, image.Url));

            return BadRequest("Can not upload file");
        }

        
        [HttpGet("images")]
        public async Task<ActionResult> GetImagesAsAdmin([FromQuery] PaginationParams paginationParams)
        {
            var images = await _unitOfWork.PhotoRepository.GetImageAsync(paginationParams);
            Response.AddPaginationHeader(images.CurrentPage, images.PageSize, images.TotalCount, images.TotalPages);
            var result = _mapper.Map<List<AdminImagesResponse>>(images.ToList());
            return Ok(result);
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

