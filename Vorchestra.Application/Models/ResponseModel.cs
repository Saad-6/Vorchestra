namespace Vorchestra.Application.Models;

public class ResponseModel<T> 
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public ResponseModel()
    {
        Success = true;
        Message = string.Empty;
    }
    public ResponseModel(bool success, string message, T data)
    {
        Success = success;
        Message = message;
        Data = data;
    }
}
