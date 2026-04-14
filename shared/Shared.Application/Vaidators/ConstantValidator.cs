using System.Collections.Concurrent;
using System.Reflection;

namespace Shared.Application.Vaidators;

public class ConstantValidator
{
    private static readonly ConcurrentDictionary<Type, HashSet<string>> _cache = new();

    public static void Validate<T>(string? value)
    {
        if(value == null) return;
        
        var validValues = _cache.GetOrAdd(typeof(T), static t =>
            t.GetFields(BindingFlags.Public | BindingFlags.Static)
             .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
             .Select(f => (string)f.GetRawConstantValue()!)
             .ToHashSet(StringComparer.Ordinal));

        if (!validValues.Contains(value))
        {
            throw new ArgumentException(
                $"Invalid value '{value}'. Allowed values: {string.Join(", ", validValues)}");
        }
    }
}