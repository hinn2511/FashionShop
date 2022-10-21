using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class FileExtensions
    {
        public static bool ValidateFile(IFormFile file, Dictionary<string, string> contentTypeDictionary, long maxLength)
        {
            if (file == null || file.Length <= 0)
                return false;

            if (!contentTypeDictionary.Any(ct => ct.Value == file.ContentType))
                return false;

            if (maxLength > 0) 
            {
                if (file.Length / 1024 > maxLength)
                    return false;
            }
             
            return true;
        }

        public static string ResizeImage(int newWidth, int newHeight, string stPhotoPath, bool saveAsTransparent, bool keepSourceImage)
        {
            Image imgPhoto = Image.FromFile(stPhotoPath);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            if (sourceWidth < sourceHeight)
                SwapDimension(newWidth, newHeight, sourceWidth, sourceHeight);

            int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            float nPercent = 0, nPercentW = 0, nPercentH = 0;

            nPercentW = ((float)newWidth / (float)sourceWidth);
            nPercentH = ((float)newHeight / (float)sourceHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((newWidth -
                          (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((newHeight -
                          (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto;
            if (saveAsTransparent) 
                bmPhoto = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);
            else
                bmPhoto = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);
            

            bmPhoto.SetResolution(
                            imgPhoto.HorizontalResolution,
                            imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(System.Drawing.Color.Transparent);

            grPhoto.InterpolationMode = System.Drawing.Drawing2D.
                                            InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(--destX, destY, ++destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            imgPhoto.Dispose();

            if (saveAsTransparent)
            {
                var newPath = stPhotoPath.Replace(stPhotoPath.Split(".").Last(), "png");
                bmPhoto.Save(newPath, ImageFormat.Png);
                if(!keepSourceImage)
                    DeleteFile(stPhotoPath);
                return newPath;
            }
            else 
                bmPhoto.Save(stPhotoPath, ImageFormat.Jpeg);

            return stPhotoPath;

            
        }
       
        public static void DeleteFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        public static async Task<string> SaveFile(IFormFile file)
        {
            string name = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                        + "." + file.FileName.Split(".").Last();
            var filePath = Path.Combine(Constant.UploadFolderPath, name);

            if (!Directory.Exists(Constant.UploadFolderPath))
            {
                Directory.CreateDirectory(Constant.UploadFolderPath);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, Constant.DefaultBufferSize))
            {
                await file.CopyToAsync(fileStream);
            }

            return filePath;
        }

        private static void SwapDimension(int newWidth, int newHeight, int sourceWidth, int sourceHeight)
        {
            int temp = newWidth;
            newWidth = newHeight;
            newHeight = temp;

        }

    }
}