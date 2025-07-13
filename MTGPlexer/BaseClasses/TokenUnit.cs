using System.Numerics;

namespace MTGPlexer.BaseClasses;

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
    public List<TokenUnit> ChildTokens { get; set; } = [];
    public int RecursiveDepth { get; set; }
    public TextSpan MatchSpan { get; set; }
    //public List<PropCapture> PropCaptures { get; set; } = [];

    /// <summary>
    /// A pre-processed and ordered list of all property captures for this token.
    /// This is the preferred way to iterate over captures for rendering or processing.
    /// </summary>
    public List<IndexedPropertyCapture> IndexedPropertyCaptures { get; set; } = [];

    protected TokenUnit(params string[] templateSnippets)
    {
        if (templateSnippets.Length == 0 && !TokenTypeRegistry.TokenTemplates.ContainsKey(Type))
        {
            // If children pass no arguments or call the default parameterless base constructor,
            // we assume they want to construct snippets from their ordered properties.

            templateSnippets = Type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Select(x => x.Name)
                .ToArray();
        }

        // Always check the static registry first, since constructing the template is somewhat heavy
        // (not much, but it adds up over all instantiations across large bodies of text)
        if (TokenTypeRegistry.TokenTemplates.ContainsKey(Type))
            Template = TokenTypeRegistry.TokenTemplates[Type];
        else
            Template = new(Type, templateSnippets);
    }

    public static TokenUnit InstantiateFromMatchString(Type type, TextSpan matchSpan, TokenUnit parentToken = null)
    {
        if (!type.IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"{type.Name} does not implement {nameof(TokenUnit)}");

        var tokenInstance = (TokenUnit)Activator.CreateInstance(type);
        tokenInstance.ParentToken = parentToken;
        tokenInstance.MatchSpan = matchSpan;
        tokenInstance.SetPropertiesFromMatch();

        tokenInstance.RecursiveDepth = 
            parentToken is null ? 0 
            : parentToken is TokenUnitOneOf ? parentToken.RecursiveDepth
            : parentToken.RecursiveDepth + 1;

        return tokenInstance;
    }

    public virtual void SetPropertiesFromMatch()
    {
        Template.PropCaptureSegments.ForEach(x => x.SetValueFromMatchSpan(this, MatchSpan));
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

        while (childrenAtCurrentRecursiveLevel.Any(x => x is not TokenUnitOneOf))
        {
            deepestDepth++;
            childrenAtCurrentRecursiveLevel = childrenAtCurrentRecursiveLevel.SelectMany(x => x.ChildTokens);
        }

        return deepestDepth;
    }

    public void AddPropertyCapture(RegexPropInfo regexPropInfo, TextSpan textSpan, object propVal)
    {
        var capturePosition = IndexedPropertyCaptures.Count;
        IndexedPropertyCaptures.Add(new(regexPropInfo, textSpan, propVal, capturePosition));
    }

    /// <summary>
    /// Only intended to be called by TokenClassRegistry upon startup. May be overridden by
    /// inheriting abstract classes who want to specify their own validation requirements.
    /// </summary>
    public virtual bool ValidateStructure()
    {
        if (string.IsNullOrEmpty(Template.RenderedRegexString))
            return false;

        return true;
    }

    /// <summary>
    /// Called after hydration to ensure the token conforms to expected data requirements.
    /// May be overridden by inheriting abstract classes who want to specify their own validation 
    /// requirements, and may be overriden by concrete classes for type-specific requirements.
    /// </summary>
    public virtual bool ValidateHydratedToken()
    {
        return true;
    }


    public override string ToString() => $"{Type.Name}{(MatchSpan.Source is null ? "" : $": {MatchSpan.ToStringValue()}")}";
}