namespace Raceday.Models.Interface
{
    public interface IProfileImageService
    {
       Task<string?> SaveAsync(IFormFile file);

        void Delete(string? relativePath);
    }
    public class ProfileImageService : IProfileImageService
    {
        private static readonly string[] Allowed= { ".jpg", ".jpeg", ".png", ".gif" };

        private const long MaxBytes = 2 * 1024 * 1024; // 2 MB

        private readonly IWebHostEnvironment _env;

        public ProfileImageService(IWebHostEnvironment env) => this._env = env;
        
        public async Task<string?> SaveAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if(!Allowed.Contains(extension) || file.Length > MaxBytes)
                return null;

            var folder = Path.Combine(_env.WebRootPath, "uploads", "profile");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{extension}";//never trust the file name from the client
            var fullPath = Path.Combine(folder, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/profile/{fileName}";
        }
        public void Delete(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;
            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

    }

}
