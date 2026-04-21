using Shared.Application.Models;

namespace Vorchestra.Application.Helpers;

public class Utility
{
    public static ResponseModel<string> MapResponse<T>(ResponseModel<T> source)
    {
        return new ResponseModel<string>
        {
            Success = source.Success,
            Message = source.Message
        };
    }
}
