using Microsoft.AspNetCore.StaticFiles;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace NhaXeMaiLinh.Services
{
    public class FileManager
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public FileManager(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private void SaveToDb(string FileName, string FileType, string FilePath,  string ArticleId)
        {
            FileTinTuc saveFile = new();
            saveFile.FileId = Guid.NewGuid().ToString();
            saveFile.TenFile = FileName;
            saveFile.Loai = FileType;
            saveFile.FilePath = FilePath.Substring(FilePath.IndexOf("files")).Replace("\\", "/");
            saveFile.TinTucId = ArticleId;
            
            _context.FileTinTuc.Add(saveFile);
            _context.SaveChanges();
        }

        public void DeleteFileDb(string FileId)
        {
            var file = _context.FileTinTuc.Find(FileId);
            // Remove on server (if existed)
            if (file != null)
            {
                string filePath = Path.Combine(_env.WebRootPath, file.FilePath).Replace('/', '\\');
                if (File.Exists(filePath)) File.Delete(filePath);

                // Remove on db after
                _context.FileTinTuc.Remove(file);
                _context.SaveChanges();
            }
        }

        public async Task<string> SaveArticleCoverAsync(IFormFile file, string path, string type, string articleID)
        {
            // Check the directory created or not. If not, create it!
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            // Copy that file to the selected directory
            var filePath = Path.Combine(path, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                SaveToDb(file.FileName, type, filePath, articleID);
            }
            return filePath.Substring(filePath.IndexOf("files")).Replace("\\", "/"); // return string of path with this file
        }

        public async Task<bool> SaveArticleFilesAsync(IFormFile file, string path, int size, string type, string articleID)
        {
            // If the size of file bigger than input's size
            if (file.Length > size) return false;
            // Check the directory created or not. If not, create it!
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            // Normalize the file name
            string fileName = Normalize(file.FileName);
            // Copy that file to the selected directory
            var filePath = Path.Combine(path, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                SaveToDb(file.FileName, type, filePath, articleID);
            }
            return true;
        }

        //CHECK
        public bool IsImageExtension(string ext)
        {
            var imageExtensions = new List<string> { ".jpg", ".png", ".gif" };
            return imageExtensions.Contains(ext.ToLower());
        }

        public bool IsDocumentExtension(string ext)
        {
            var documentExtension = new List<string> { ".doc", ".docx", ".pdf" };
            return documentExtension.Contains(ext.ToLower());
        }

        public string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (provider.TryGetContentType(fileName, out var contentType))
            {
                return contentType;
            }

            return "application/octet-stream";
        }

        public static string StripHtmlTags(string html)
        {
            string htmlWithoutTags = Regex.Replace(html, "<.*?>", string.Empty);
            return WebUtility.HtmlDecode(htmlWithoutTags);
        }

        private static string Normalize(string input)
        {
            return new string(input.Normalize(NormalizationForm.FormD).Where(c => c < 128).ToArray()); // Turn "Unicode" into "ASCII"
        }
    }
}
