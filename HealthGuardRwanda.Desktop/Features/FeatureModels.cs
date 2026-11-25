using HealthGuardRwanda.Core.Controllers;

namespace HealthGuardRwanda.Desktop.Features;

public sealed class FeatureParameter
{
    public FeatureParameter(string name, string? picker = null, string defaultValue = "")
    {
        Name = name;
        Picker = picker;
        DefaultValue = defaultValue;
    }

    public string Name { get; }
    public string? Picker { get; }
    public string DefaultValue { get; }
    public string Label => string.Join(" ", Name.Split('_').Select(w => string.IsNullOrEmpty(w) ? w : char.ToUpper(w[0]) + w[1..]));
}

public sealed class Feature
{
    public Feature(
        string id,
        string title,
        string group,
        string description,
        IReadOnlyList<FeatureParameter> parameters,
        Func<IReadOnlyDictionary<string, string>, ServiceResult> execute)
    {
        Id = id;
        Title = title;
        Group = group;
        Description = description;
        Parameters = parameters;
        Execute = execute;
    }

    public string Id { get; }
    public string Title { get; }
    public string Group { get; }
    public string Description { get; }
    public IReadOnlyList<FeatureParameter> Parameters { get; }
    public Func<IReadOnlyDictionary<string, string>, ServiceResult> Execute { get; }
}
