using LKaifer_BugTracker.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LKaifer_BugTracker.Helpers
{
    public static class FileUploadHelper
    {
        public static string UploadSingleFile(HttpPostedFileBase file)
        {
            if (FileUploadValidator.IsWebFriendlyImage(file) || FileUploadValidator.IsWebFriendlyFile(file))
            {
                var fileName = Path.GetFileName(file.FileName);
                file.SaveAs(Path.Combine(HttpContext.Current.Server.MapPath("/Uploads/"), fileName));
                return "/Uploads/" + fileName;
            }
            return "";
        }

        public static List<Tuple<string, string, string>> UploadMultipleFiles(HttpPostedFileBase[] files)
        {
            List<Tuple<string, string, string>> resultList = new List<Tuple<string, string, string>>(); 
            if (files != null)
            {
                if (files.Length > 0)
                {
                    foreach(var file in files)
                    {
                        if (FileUploadValidator.IsWebFriendlyImage(file) || FileUploadValidator.IsWebFriendlyFile(file))
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var iconName = ImageHelpers.GetIconPath(fileName);
                            file.SaveAs(Path.Combine(HttpContext.Current.Server.MapPath("/Uploads/Attachments/"), fileName));
                            resultList.Add(new Tuple<string, string, string>(fileName, "/Uploads/Attachments/" + fileName, iconName));
                        }
                    }
                }
            }
            return resultList;
        }
    }
}