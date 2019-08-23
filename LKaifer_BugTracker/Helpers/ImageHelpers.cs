using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LKaifer_BugTracker.Helpers
{
    public static class ImageHelpers
    {
        public static string GetIconPath(string fileName)
        {
            switch(Path.GetExtension(fileName))
            {
                case ".pdf":
                    return "fa-file-pdf-o";
                case ".doc":
                    return "fa-file-word-o";
                case ".docx":
                    return "fa-file-word-o";
                case "xls":
                    return "fa-file-excel-o";
                case "xlsx":
                    return "fa-file-excel-o";
                case ".zip":
                    return "fa-file-zip-o";
                default:
                    return "fa-file-o";
            }
        }
    }
}