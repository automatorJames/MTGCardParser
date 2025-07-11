namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DistilledValueAttribute : Attribute
{
    public string DistilledFromPropName { get; set; }

    public DistilledValueAttribute()
    {
    }

    public DistilledValueAttribute(string distilledFromPropName)
    {
        DistilledFromPropName = distilledFromPropName;
    }

    public PropertyInfo GetDistilledFromProp(PropertyInfo prop)
    {
        if (!string.IsNullOrEmpty(DistilledFromPropName))
            return prop.DeclaringType.GetProperty(DistilledFromPropName);

        var placeholderCaptures = prop.DeclaringType
            .GetProperties().Where(x => x.PropertyType == typeof(PlaceholderCapture))
            .ToList();

        if (placeholderCaptures.Count() != 1)
            throw new Exception($"Distilled values must either declare a distilled-from property, or have exactly one PlaceholderCapture property");

        return placeholderCaptures.First();
    }
}

