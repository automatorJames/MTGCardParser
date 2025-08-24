namespace MTGPlexer.BaseClasses;

public abstract class TokenUnitDistilled : TokenUnit
{
    public Dictionary<RegexPropInfo, Dictionary<RegexPropInfo, object>> DistilledValues { get; } = [];
    protected TokenUnitDistilled(params string[] templateSnippets) : base(templateSnippets) { }

    public abstract void SetComplexValuesFromMatch();

    public override void SetPropertiesFromMatch()
    {
        // First, allow the base class to set all properties normally
        base.SetPropertiesFromMatch();

        // Second, apply whatever class-specific decomposition is necessary
        SetComplexValuesFromMatch();

        // Third, register all the non-default distilled prop values for lookup reference
        RegisterDistilledPropVals();
    }

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

                if (
                       distilledProp.Prop.PropertyType.IsValueType
                       && !distilledProp.UnderlyingType.IsEnum
                       && val.Equals(Activator.CreateInstance(distilledProp.Prop.PropertyType))
                   ) continue;

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
    public override bool ValidateStructure()
    {
        var placeholderCaptureProps = GetPlaceholderCaptureProps();
        var isSinglePlaceholder = placeholderCaptureProps.Count == 1;
        var distilledProps = GetDistilledProps();

        if (!distilledProps.Any())
            return false;

        if (isSinglePlaceholder)
            return true;

        foreach (var prop in distilledProps)
        {
            var propName = prop.Prop.GetCustomAttribute<DistilledValueAttribute>()?.DistilledFromPropName;

            if (propName is null)
                return false;

            var distilledFromProp = Type.GetProperty(propName);

            if (distilledFromProp is null)
                return false;
        }

        return true;
    }
}

