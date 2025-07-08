namespace MTGCardParser.BaseClasses;

public abstract class TokenUnit
{
    static string RegexTemplatePropName = "RegexTemplate";

    Type _type;
    public Type Type
    {
        get
        {
            if (_type is null)
                _type = GetType();

            return _type;
        }
    }

    Dictionary<PropertyInfo, object> _distilledValues;
    public Dictionary<PropertyInfo, object> DistilledValues
    {
        get
        {
            if (_distilledValues is null)
                _distilledValues = GetDistilledValues();

            return _distilledValues;
        }
    }

    public TokenUnit ParentToken { get; set; }
    public List<TokenUnit> ChildTokens { get; set; } = new();
    public int RecursiveDepth { get; set; }
    public TextSpan MatchSpan { get; set; }

    // This now uses the new, robust PropertyCapture class as its key.
    public Dictionary<PropertyCapture, TextSpan> PropMatches { get; set; } = new();

    // This remains the source for the UI layer.
    public List<IndexedPropertyCapture> OrderedPropCaptures { get; private set; } = [];

    public RegexTemplate GetRegexTemplate()
    {
        if (TokenClassRegistry.IsInitialized)
            return TokenClassRegistry.TypeRegexTemplates[Type];

        var prop = GetType().GetProperty(RegexTemplatePropName);

        if (prop is null)
            throw new Exception($"{GetType().Name} doesn't contain a property named {RegexTemplatePropName})");

        return prop.GetValue(this) as RegexTemplate;
    }

    /// <summary>
    /// Creates and fully hydrates a TokenUnit instance from a given text span.
    /// This method now orchestrates the entire process, delegating the complex
    /// hydration logic to the RegexTemplate.
    /// </summary>
    public static TokenUnit InstantiateFromMatchString(Type type, TextSpan matchSpan, TokenUnit parentToken = null)
    {
        if (!type.IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"{type.Name} does not implement {nameof(TokenUnit)}");

        var instance = (TokenUnit)Activator.CreateInstance(type);
        instance.ParentToken = parentToken;
        instance.RecursiveDepth = parentToken is null ? 0 : parentToken.RecursiveDepth + 1;
        instance.MatchSpan = matchSpan;

        // 1. Get the template and the corresponding static regex.
        var template = instance.GetRegexTemplate();
        var regex = TokenClassRegistry.TypeRegexes[type];

        // 2. Perform the match to get group information.
        var match = regex.Match(matchSpan.ToStringValue());
        if (!match.Success)
            throw new InvalidOperationException($"'{matchSpan.ToStringValue()}' did not successfully match the regex for type '{type.Name}'. This should not happen if the tokenizer is configured correctly.");

        // 3. Delegate hydration to the template. This populates all properties and PropMatches.
        template.HydrateInstance(instance, match);

        // 4. Build the ordered list for the UI, now that PropMatches is populated.
        instance.OrderedPropCaptures = instance.PropMatches
            .Select((kvp, index) => new IndexedPropertyCapture(kvp.Key, kvp.Value, index))
            .OrderBy(capture => capture.Span.Position.Absolute)
            .ToList();

        return instance;
    }

    Dictionary<PropertyInfo, object> GetDistilledValues(bool ignoreDefaultVals = true)
    {
        Dictionary<PropertyInfo, object> dict = new();
        var distilledProps = Type.GetProperties().Where(x => x.GetCustomAttribute<DistilledValueAttribute>() is not null);

        foreach (var distilledProp in distilledProps)
        {
            var val = distilledProp.GetValue(this);

            if (distilledProp.PropertyType.IsValueType && val.Equals(Activator.CreateInstance(distilledProp.PropertyType)))
                continue;

            dict[distilledProp] = val;
        }

        return dict;
    }

    public int GetDeepestChildLevel()
    {
        IEnumerable<TokenUnit> childrenAtCurrentRecursiveLevel = ChildTokens;
        var deepestDepth = 0;

        while (childrenAtCurrentRecursiveLevel.Any())
        {
            deepestDepth++;
            childrenAtCurrentRecursiveLevel = childrenAtCurrentRecursiveLevel.SelectMany(x => x.ChildTokens);
        }

        return deepestDepth;
    }
}