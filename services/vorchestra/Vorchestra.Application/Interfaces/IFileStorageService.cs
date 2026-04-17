namespace Vorchestra.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName);
    void DeleteFile(string fileName);
}