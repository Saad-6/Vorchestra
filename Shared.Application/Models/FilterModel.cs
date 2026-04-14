using System.Linq.Expressions;

namespace Shared.Application.Models;

public class FilterModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Dictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();

    public IQueryable<T> ApplyFilters<T>(IQueryable<T> query)
    {
        var parameter = Expression.Parameter(typeof(T), "x");

        foreach (var filter in Filters)
        {
            var property = Expression.Property(parameter, filter.Key);
            var constant = Expression.Constant(filter.Value);
            var equality = Expression.Equal(property, constant);

            var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            query = query.Where(lambda);
        }

        query = query.Skip((Page - 1) * PageSize).Take(PageSize);

        return query;
    }
}