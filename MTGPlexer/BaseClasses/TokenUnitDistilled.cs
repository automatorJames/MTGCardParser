namespace MTGPlexer.BaseClasses;

public abstract class TokenUnitDistilled : TokenUnit
{
    public Dictionary<RegexPropInfo, Dictionary<RegexPropInfo, object>> DistilledValues { get; } = [];
    protected TokenUnitDistilled(params string[] templateSnippets) : base(templateSnippets) { }

    public abstract void SetComplexValuesFromMatch();

    /// <summary>
    /// For each distilled value property associated with each placeholder property for this type,
    /// set the value on this object to the DistilledValues dictionary to ease external lookup.
    /// </summary>
    protected virtual void RegisterDistilledPropVals()
    {
        foreach (var placeholderPropItem in TokenTypeRegistry.DistilledProperties[Type])
            foreach (var distilledProp in placeholderPropItem.Value)
            {
                var val = distilledProp.Prop.GetValue(this);

                if (val is null)
                    continue;

                if (!DistilledValues.ContainsKey(placeholderPropItem.Key))
                    DistilledValues[placeholderPropItem.Key] = [];

                DistilledValues[placeholderPropItem.Key][distilledProp] = val;
            }
    }

    public List<PropertyInfo> GetPlaceholderCaptureProps() =>
        Type.GetProperties().Where(x => x.PropertyType == typeof(PlaceholderCapture)).ToList();

    public List<RegexPropInfo> GetDistilledProps() =>
        Type.GetProperties()    
        .Where(x => x.IsDefined(typeof(DistilledValueAttribute)))
        .Select(x => new RegexPropInfo(x))
        .ToList();

    public Dictionary<RegexPropInfo, List<RegexPropInfo>> GetDistilledPropAssociations()
    {
        Dictionary<RegexPropInfo, List<RegexPropInfo>> dict = [];
        var distilledProps = GetDistilledProps();
        var placeholderCaptureProps = GetPlaceholderCaptureProps();
        var isSinglePlaceholder = placeholderCaptureProps.Count == 1;

        foreach (var distilledProp in distilledProps)
        {
            PropertyInfo distilledFromProp = null;
            var distilledFromPropName = distilledProp.Prop.GetCustomAttribute<DistilledValueAttribute>()?.DistilledFromPropName;

            // If the distilled prop has a DistilledValueAttribute with a defined DistilledFromPropName, get that prop
            if (distilledFromPropName is not null)
                distilledFromProp = Type.GetProperty(distilledFromPropName);

            // If not attribute is defined, we expect to find exactly one placeholder capture prop, so get that one
            if (distilledFromProp is null && placeholderCaptureProps.Count == 1)
                distilledFromProp = placeholderCaptureProps[0];

            if (distilledFromProp is null)
                throw new Exception($"Distilled values must either declare a distilled-from property, or be a property of a type with exactly one PlaceholderCapture property");

            var distilledFromPropRegexInfo = new RegexPropInfo(distilledFromProp);

            if (!dict.TryGetValue(distilledFromPropRegexInfo, out var list))
            {
                list = [];
                dict[distilledFromPropRegexInfo] = list;
            }

            list.Add(distilledProp);
        }

        return dict;
    }

    /// <summary>
    /// Only intended to be called by TokenClassRegistry upon startup.
    /// </summary>
    public override string ValidateStructure()
    {
        var placeholderCaptureProps = GetPlaceholderCaptureProps();
        var isSinglePlaceholder = placeholderCaptureProps.Count == 1;
        var distilledProps = GetDistilledProps();

        if (!distilledProps.Any())
            return $"{nameof(TokenUnitDistilled)} must be at least one distilled prop";

        if (isSinglePlaceholder)
            return null;

        foreach (var prop in distilledProps)
        {
            var propName = prop.Prop.GetCustomAttribute<DistilledValueAttribute>()?.DistilledFromPropName;

            if (propName is null)
                return $"Prop '{prop.Prop.Name}' isn't associated with any {nameof(DistilledValueAttribute.DistilledFromPropName)}";

            var distilledFromProp = Type.GetProperty(propName);

            if (distilledFromProp is null)
                return $"Prop '{propName}' isn't defined on type '{Type.Name}'";
        }

        return null;
    }
}

