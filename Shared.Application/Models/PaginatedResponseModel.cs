namespace Shared.Application.Models
{
    public class PaginatedResponseModel<T> : ResponseModel<List<T>>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }
}
