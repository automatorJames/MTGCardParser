namespace MTGCardParser.BaseClasses;

public abstract class TokenUnit
{
    public RegexTemplate Template { get; init; }

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
    public Dictionary<RegexPropInfo, TextSpan> PropMatches { get; set; } = new(new CapturePropComparer());

    /// <summary>
    /// A pre-processed and ordered list of all property captures for this token.
    /// This is the preferred way to iterate over captures for rendering or processing.
    /// </summary>
    public List<IndexedPropertyCapture> OrderedPropCaptures { get; private set; } = [];

    protected TokenUnit(params object[] templateSnippets)
    {
        if (templateSnippets.Length == 0)
        {
            // If children pass no arguments or call the default parameterless base constructor,
            // we assume they want to construct snippets from their ordered properties.
            
            templateSnippets = Type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Select(x => x.Name)
                .ToArray();
        }

        if (TokenClassRegistry.TokenTemplates.ContainsKey(Type))
            Template = TokenClassRegistry.TokenTemplates[Type];
        else
            Template = new(Type, templateSnippets);
    }

    public static TokenUnit InstantiateFromMatchString(Type type, TextSpan matchSpan, TokenUnit parentToken = null)
    {
        if (!type.IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"{type.Name} does not implement {nameof(TokenUnit)}");

        var tokenInstance = (TokenUnit)Activator.CreateInstance(type);
        tokenInstance.ParentToken = parentToken;
        tokenInstance.RecursiveDepth = parentToken is null ? 0 : parentToken.RecursiveDepth + 1;
        tokenInstance.MatchSpan = matchSpan;
        tokenInstance.SetPropertiesFromMatch();

        return tokenInstance;
    }

    public virtual void SetPropertiesFromMatch()
    {
        Template.PropCaptureSegments.ForEach(x => x.SetValueFromMatchSpan(this, MatchSpan));

        foreach (var alternativeCaptureSet in Template.AlternativePropCaptureSets)
            if (!alternativeCaptureSet.Alternatives.Any(x => x.SetValueFromMatchSpan(this, MatchSpan)))
                throw new Exception($"Match string '{MatchSpan.ToStringValue()}' was passed to an alternative set, but no alternative was matched");

        // After PropMatches is fully populated, create the ordered and indexed list one time.
        var propMatchesAsList = PropMatches.ToList(); // Create a stable list to get an index.

        OrderedPropCaptures = propMatchesAsList
            .Select((kvp, index) => new IndexedPropertyCapture(kvp.Key, kvp.Value, index))
            .OrderBy(capture => capture.Span.Position.Absolute)
            .ToList();
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