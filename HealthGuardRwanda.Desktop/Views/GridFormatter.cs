using System.Data;
using System.Reflection;
using HealthGuardRwanda.Core.Models;

namespace HealthGuardRwanda.Desktop.Views;

public static class GridFormatter
{
    public static DataTable ToTable(object? data, out string extra)
    {
        extra = string.Empty;
        var table = new DataTable();

        if (data is null)
            return table;

        if (data is Dictionary<string, long> summary)
        {
            table.Columns.Add("Key");
            table.Columns.Add("Value");
            foreach (var pair in summary)
                table.Rows.Add(pair.Key, pair.Value);
            return table;
        }

        if (data is List<object[]> rows)
        {
            table.Columns.Add("Status");
            table.Columns.Add("Count");
            foreach (var row in rows)
                table.Rows.Add(row.Length > 0 ? row[0] : "", row.Length > 1 ? row[1] : 0);
            return table;
        }

        var pageItemsProperty = data.GetType().GetProperty("Items");
        if (pageItemsProperty is not null)
        {
            extra = $"Total elements: {Get(data, "TotalElements")}   Total pages: {Get(data, "TotalPages")}   Page: {Get(data, "Page")}";
            data = pageItemsProperty.GetValue(data);
        }

        var items = (data as System.Collections.IEnumerable)?.Cast<object>().ToList() ?? new List<object>();
        if (data is not System.Collections.IEnumerable)
            items = new List<object> { data! };

        if (items.Count == 0)
            return table;

        var properties = items[0].GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToArray();

        foreach (var property in properties)
            table.Columns.Add(property.Name);

        foreach (var item in items)
        {
            var values = properties.Select(p => CellValue(item, p)).ToArray();
            table.Rows.Add(values);
        }

        return table;
    }

    private static object? Get(object target, string name) => target.GetType().GetProperty(name)?.GetValue(target);

    private static object CellValue(object owner, PropertyInfo property)
    {
        var value = property.GetValue(owner);
        return value switch
        {
            null => "",
            string s => s,
            Guid g => g.ToString(),
            DateOnly d => d.ToString("yyyy-MM-dd"),
            EnrollmentStatus status => status.ToString(),
            Citizen c => $"{c.FirstName} {c.LastName} ({c.NationalId})",
            EnrollmentOfficer o => $"{o.FirstName} {o.LastName} ({o.Email})",
            Sector s => $"{s.SectorNumber} - {s.Name}",
            Enrollment e => e.Id.ToString(),
            List<HouseholdCategory> categories => string.Join(", ", categories.Select(c => c.Name)),
            List<EnrollmentOfficer> officers => string.Join(", ", officers.Select(o => $"{o.FirstName} {o.LastName}")),
            List<Enrollment> enrollments => string.Join(", ", enrollments.Select(e => $"{e.EnrollmentDate:yyyy-MM-dd} {e.Status}")),
            bool b => b,
            int i => i,
            long l => l,
            _ => value.ToString() ?? ""
        };
    }
}
