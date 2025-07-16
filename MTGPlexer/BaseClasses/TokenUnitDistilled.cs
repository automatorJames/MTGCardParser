namespace MTGPlexer.BaseClasses;

public abstract class TokenUnitDistilled : TokenUnit
{
    public Dictionary<PropertyInfo, Dictionary<PropertyInfo, object>> DistilledValues { get; } = [];
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
            foreach (var distilledProp in (List<PropertyInfo>)placeholderPropItem.Value)
            {
                var val = distilledProp.GetValue(this);

                if (val is null)
                    continue;

                if (
                       distilledProp.PropertyType.IsValueType
                       && !distilledProp.UnderlyingType().IsEnum
                       && val.Equals(Activator.CreateInstance(distilledProp.PropertyType))
                   ) continue;

                if (!DistilledValues.ContainsKey(placeholderPropItem.Key))
                    DistilledValues[placeholderPropItem.Key] = [];

                DistilledValues[placeholderPropItem.Key][distilledProp] = val;
            }
    }

    public List<PropertyInfo> GetPlaceholderCaptureProps() =>
        Type.GetProperties().Where(x => x.PropertyType == typeof(PlaceholderCapture)).ToList();

    public List<PropertyInfo> GetDistilledProps() =>
        Type.GetProperties().Where(x => x.IsDefined(typeof(DistilledValueAttribute))).ToList();

    public Dictionary<PropertyInfo, List<PropertyInfo>> GetDistilledPropAssociations()
    {
        Dictionary<PropertyInfo, List<PropertyInfo>> dict = [];
        var distilledProps = GetDistilledProps();
        var placeholderCaptureProps = GetPlaceholderCaptureProps();
        var isSinglePlaceholder = placeholderCaptureProps.Count == 1;

        foreach (var prop in distilledProps)
        {
            PropertyInfo distilledFromProp = null;
            var propName = prop.GetCustomAttribute<DistilledValueAttribute>()?.DistilledFromPropName;

            if (propName is not null)
                distilledFromProp = Type.GetProperty(propName);

            if (distilledFromProp is null && placeholderCaptureProps.Count > 0)
                distilledFromProp = placeholderCaptureProps[0];

            if (distilledFromProp is null)
                throw new Exception($"Distilled values must either declare a distilled-from property, or be a property of a type with exactly one PlaceholderCapture property");

            if (!dict.TryGetValue(distilledFromProp, out var list))
            {
                list = [];
                dict[distilledFromProp] = list;
            }

            list.Add(prop);
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
            var propName = prop.GetCustomAttribute<DistilledValueAttribute>()?.DistilledFromPropName;

            if (propName is null)
                return false;

            var distilledFromProp = Type.GetProperty(propName);

            if (distilledFromProp is null)
                return false;
        }

        return true;
    }
}

