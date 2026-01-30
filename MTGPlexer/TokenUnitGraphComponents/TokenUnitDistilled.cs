namespace MTGPlexer.TokenUnitGraphComponents;

public abstract class TokenUnitDistilled : TokenUnit
{
    /// <summary>
    /// Dictionary to aid in capture analysis. Allows external callers to quickly associate each
    /// PlaceholderCapture property with its distilled properties.
    /// </summary>
    public Dictionary<PropertyInfo, List<PropertyInfo>> PropDistillationMap{ get; private set; } = [];

    /// <summary>
    /// Dictionary to aid in capture analysis. Similar to the PropDistillationMap dictionary, except 
    /// holds concrete distilled values for each PlaceholderCapture on this instance.
    /// </summary>
    public Dictionary<PropertyInfo, Dictionary<PropertyInfo, object>> DistilledVals{ get; private set; } = [];

    public abstract void DistillValuesFromPlaceholders();

    protected override void OnAfterHydrated()
    {
        RegisterDistilledProps();
        DistillValuesFromPlaceholders();
        RegisterDistilledPropVals();
    }

    /// <summary>
    /// For each distilled value property associated with each placeholder property for this type,
    /// set the value on this object to the DistilledValues dictionary to ease external lookup.
    /// If this type has already been constructed, the dictionary is globally cached for performance.
    /// </summary>
    void RegisterDistilledProps()
    {
        if (TokenTypeRegistry.PropDistillationMaps.TryGetValue(Type, out var cachedMap))
        {
            PropDistillationMap = cachedMap;
            return;
        }

        PropDistillationMap = GetDistilledPropAssociations();
        TokenTypeRegistry.PropDistillationMaps[Type] = PropDistillationMap;
    }

    /// <summary>
    /// Populates the DistilledVals dictionary so callers can easily map PlaceholderProp -> DistilledProps -> non-null values.
    /// Only really useful for analytics. Not strictly necessary for using the TokenUnitDistilled instance in a game engine.
    /// </summary>
    public void RegisterDistilledPropVals()
    {
        foreach (var (placeholderProp, distilledPropList) in PropDistillationMap)
        {
            foreach (var distilledProp in distilledPropList)
            {
                var distilledVal = distilledProp.GetValue(this);

                if (distilledVal is null)
                    continue;

                DistilledVals.TryAdd(placeholderProp, []);
                DistilledVals[placeholderProp][distilledProp] = distilledVal;
            }
        }
    }

    List<PropertyInfo> GetPlaceholderCaptureProps() =>
        Type.GetProperties().Where(x => x.PropertyType == typeof(PlaceholderCapture)).ToList();

    List<PropertyInfo> GetDistilledProps() =>
        Type.GetProperties()    
        .Where(x => x.IsDefined(typeof(DistilledValueAttribute)))
        .ToList();

    Dictionary<PropertyInfo, List<PropertyInfo>> GetDistilledPropAssociations()
    {
        Dictionary<PropertyInfo, List<PropertyInfo>> dict = [];
        var distilledProps = GetDistilledProps();
        var placeholderCaptureProps = GetPlaceholderCaptureProps();
        var isSinglePlaceholder = placeholderCaptureProps.Count == 1;

        foreach (var distilledProp in distilledProps)
        {
            PropertyInfo distilledFromProp = null;
            var distilledFromPropName = distilledProp.GetCustomAttribute<DistilledValueAttribute>()?.DistilledFromPropName;

            // If the distilled prop has a DistilledValueAttribute with a defined DistilledFromPropName, get that prop
            if (distilledFromPropName is not null)
                distilledFromProp = Type.GetProperty(distilledFromPropName);

            // If not attribute is defined, we expect to find exactly one placeholder capture prop, so get that one
            if (distilledFromProp is null && placeholderCaptureProps.Count == 1)
                distilledFromProp = placeholderCaptureProps[0];

            if (distilledFromProp is null)
                throw new Exception($"Distilled values must either declare a distilled-from property, or be a property of a type with exactly one PlaceholderCapture property");

            if (!dict.TryGetValue(distilledFromProp, out var list))
            {
                list = [];
                dict[distilledFromProp] = list;
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
            var propName = prop.GetCustomAttribute<DistilledValueAttribute>()?.DistilledFromPropName;

            if (propName is null)
                return $"Prop '{prop.Name}' isn't associated with any {nameof(DistilledValueAttribute.DistilledFromPropName)}";

            var distilledFromProp = Type.GetProperty(propName);

            if (distilledFromProp is null)
                return $"Prop '{propName}' isn't defined on type '{Type.Name}'";
        }

        return null;
    }
}

