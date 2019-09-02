using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;


namespace MicroAssignment.Helpers
{
    public class ImageHelper
    {
        // folder for the upload
        public static readonly string ItemUploadFolderPath = "~/Uploads/Items/";
       

        public static bool renameUploadFile(HttpPostedFileBase file, Int32 counter = 0)
        {
            var fileName = Path.GetFileName(file.FileName);

            string append = "item_";
            string finalFileName = append + ((counter).ToString()) + "_" + fileName;
            if (System.IO.File.Exists
        (HttpContext.Current.Request.MapPath(ItemUploadFolderPath + fileName)))
            {
                //file exists 
                return renameUploadFile(file, ++counter);
            }
            //file doesn't exist, upload item but validate first
            
            return uploadFile(file, finalFileName);
        }

        private static bool uploadFile(HttpPostedFileBase file, string finalFileName)
        {
            var path =
          Path.Combine(HttpContext.Current.Request.MapPath(ItemUploadFolderPath), finalFileName);
            string extension = Path.GetExtension(file.FileName);

            //make sure the file is valid
            if (!validateExtension(extension))
            {
                return false;
            }

            try
            {
                file.SaveAs(path);

                Image imgOriginal = Image.FromFile(path);

                //pass in whatever value you want for the width (180)
                Image imgActual = ScaleBySize(imgOriginal, 600);
                imgOriginal.Dispose();
                imgActual.Save(path);
                imgActual.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool validateExtension(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return true;
                case ".png":
                    return true;
                case ".gif":
                    return true;
                case ".jpeg":
                    return true;
                case ".xls":
                    return true;
                case ".xlsx":
                    return true;
                case ".pdf":
                    return true;
                case ".doc":
                    return true;
                case ".docx":
                    return true;
                default:
                    return false;
            }
        }


        public static Image ScaleBySize(Image imgPhoto, int size)
        {
            int logoSize = size;

            float sourceWidth = imgPhoto.Width;
            float sourceHeight = imgPhoto.Height;
            float destHeight = 0;
            float destWidth = 0;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            // Resize Image to have the height = logoSize/2 or width = logoSize.
            // Height is greater than width, set Height = logoSize and resize width accordingly
            if (sourceWidth > (2 * sourceHeight))
            {
                destWidth = logoSize;
                destHeight = (float)(sourceHeight * logoSize / sourceWidth);
            }
            else
            {
                int h = logoSize / 2;
                destHeight = h;
                destWidth = (float)(sourceWidth * h / sourceHeight);
            }
            // Width is greater than height, set Width = logoSize and resize height accordingly

            Bitmap bmPhoto = new Bitmap((int)destWidth, (int)destHeight,
                                        PixelFormat.Format32bppPArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, (int)destWidth, (int)destHeight),
                new Rectangle(sourceX, sourceY, (int)sourceWidth, (int)sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();

            return bmPhoto;
        }
    }
}