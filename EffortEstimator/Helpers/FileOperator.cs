using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EffortEstimator.Helpers
{
    public static class FileOperator
    {
        private static readonly string rootPath = "wwwroot/uploads/";


        public static async Task<bool> UploadFile(string channelName, IFormFile file)
        {
            string channelPath = Path.Combine(rootPath, channelName);
            if (!Directory.Exists(channelPath))
                Directory.CreateDirectory(channelPath);

            if (file.Length > 0)
            {
                string filePath = Path.Combine(channelPath, file.FileName);
                using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
            }

            return true;
        }

        public static Tuple<bool, string> CheckIfExist(string channelName)
        {
            string channelPath = Path.Combine(rootPath, channelName);
            if (!Directory.Exists(channelPath))
                return new Tuple<bool, string>(false, "");

            if (Directory.GetFiles(channelPath).Length == 0)
                return new Tuple<bool, string>(false, "");

            return new Tuple<bool, string>(true, Path.GetFileName(Directory.GetFiles(channelPath).ElementAt(0)));
        }

        public static string GetFilePath(string channelName)
        {
            string channelPath = Path.Combine(rootPath, channelName);
            if (!Directory.Exists(channelPath))
                throw new Exception("File not found!");

            if (Directory.GetFiles(channelPath).Length == 0)
                throw new Exception("File not found!");

            return Directory.GetFiles(channelPath).ElementAt(0);
        }
    }
}
