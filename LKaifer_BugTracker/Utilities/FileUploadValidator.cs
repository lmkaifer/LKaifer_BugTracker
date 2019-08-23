using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace LKaifer_BugTracker.Utilities
{
    public static class FileUploadValidator
    {
        public static bool IsWebFriendlyImage(HttpPostedFileBase image)
        {
            //check for actual object
            if (image == null)
                return false;

            //check size - file must be less than 2 MB and greater than 1 KB
            if (image.ContentLength > 2 * 1024 * 1024 || image.ContentLength < 1024)
                return false;

            try
            {
                using (var img = Image.FromStream(image.InputStream))
                {
                    return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                           ImageFormat.Png.Equals(img.RawFormat) ||
                           ImageFormat.Gif.Equals(img.RawFormat);
                }
            }
            catch
            {
                return false;
            }

        }
        public static bool IsWebFriendlyFile(HttpPostedFileBase file)
        {
            //check for actual object
            if (file == null)
                return false;

            //check size - file must be less than 2 MB and greater than 1 KB
            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                return false;

            try
            {
                string ext = Path.GetExtension(file.FileName).ToLower();
                if (ext == ".pdf" || ext == ".doc" || ext == ".docx" || ext == ".xls" || ext == ".xlsx" || ext == ".zip")
                    return true;
                return false;
            }
            catch
            {
                return false;
            }

        }
    }
}
