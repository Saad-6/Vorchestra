using Microsoft.AspNetCore.Hosting;
using Vorchestra.Application.Interfaces;

namespace Vochestra.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    public FileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public void DeleteFile(string fileName)
    {
        var filePath = Path.Combine(_env.ContentRootPath, "uploads", "projects", fileName);
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to delete file: {fileName}", ex);
            }
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName)
    {
        var uploadDir = Path.Combine(_env.ContentRootPath, "uploads", "projects");
        Directory.CreateDirectory(uploadDir);

        var uniqueFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
        var filePath = Path.Combine(uploadDir, uniqueFileName);

        await using var destination = File.Create(filePath);
        await fileStream.CopyToAsync(destination);

        return filePath;
    }
}
