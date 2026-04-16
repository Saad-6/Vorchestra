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
            var targetType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;

            // ✅ HANDLE NULL FIRST
            if (filter.Value == null)
            {
                // Only allow nullable types or reference types
                if (property.Type.IsValueType && Nullable.GetUnderlyingType(property.Type) == null)
                {
                    continue;
                }
                if(Nullable.GetUnderlyingType(property.Type) == null)
                {
                    continue;
                }
                var nullConstant = Expression.Constant(null, property.Type);
                var nullCheck = Expression.Equal(property, nullConstant);

                var lambdaNull = Expression.Lambda<Func<T, bool>>(nullCheck, parameter);
                query = query.Where(lambdaNull);

                continue;
            }
            if (property.Type == typeof(string) && string.IsNullOrWhiteSpace(filter.Value?.ToString()))
            {
                continue;
            }
            // Convert safely
            object typedValue;

            if (targetType.IsEnum)
                typedValue = Enum.Parse(targetType, filter.Value.ToString());
            else if (targetType == typeof(Guid))
                typedValue = Guid.Parse(filter.Value.ToString());
            else
                typedValue = Convert.ChangeType(filter.Value, targetType);

            var constant = Expression.Constant(typedValue, property.Type);

            var equality = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            query = query.Where(lambda);
        }

        query = query.Skip((Page - 1) * PageSize).Take(PageSize);

        return query;
    }
}