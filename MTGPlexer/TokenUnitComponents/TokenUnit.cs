namespace MTGPlexer.TokenUnitComponents;

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

    public TokenUnit ParentToken { get; set; }
    public RegexPropInfo ParentTokenProp { get; set; }
    public Match TopLevelMatch { get; set; }
    public Capture Capture { get; set; }
    public string Path { get; set; }

    /// <summary>
    /// A pre-processed and ordered list of all property captures for this token.
    /// This is the preferred way to iterate over captures for rendering or processing.
    /// </summary>
    public List<IndexedPropertyCapture> IndexedPropertyCaptures { get; set; } = [];

    protected TokenUnit(params string[] templateSnippets)
    {
        if (templateSnippets.Length == 0 && !TokenTypeRegistry.Templates.ContainsKey(Type))
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
        if (TokenTypeRegistry.Templates.ContainsKey(Type))
            Template = TokenTypeRegistry.Templates[Type];
        else
            Template = new(Type, templateSnippets);

        OnInitialized();
    }

    protected virtual void OnInitialized()
    {
        // Base implementation requires no initialization
    }

    protected virtual void OnAfterHydrated()
    {
        // Base implementation requires no actions post-hydration
    }

    public List<TokenUnit> GetChildTokens() => IndexedPropertyCaptures
        .Where(x => x.IsChildToken)
        .Select(x => x.Value)
        .OfType<TokenUnit>()
        .ToList();

    public static TokenUnit HydrateFromMatch(Type tokenUnitType, Match match, Capture childCapture = null)
    {
        var tokenUnit = (TokenUnit)Activator.CreateInstance(tokenUnitType);
        tokenUnit.TopLevelMatch = match;
        tokenUnit.Capture = childCapture ?? match;
        tokenUnit.Path = $"{match.Index}-{tokenUnitType.Name}"; // Start as root index + type name (may child later if assigned as child)

        foreach (var captureProp in tokenUnit.Template.CaptureGroupProps)
            if (match.Groups[captureProp.Name].Success)
                captureProp.SetValueFromMatch(tokenUnit, match);

        tokenUnit.OnAfterHydrated();

        return tokenUnit;
    }

    public void SetPropertyFromCapture(RegexPropInfo regexPropInfo, Capture capture, object propVal)
    {
        regexPropInfo.Prop.SetValue(this, propVal);
        var capturePosition = IndexedPropertyCaptures.Count;
        IndexedPropertyCaptures.Add(new(regexPropInfo, capture, propVal, capturePosition, Path));

        if (propVal is TokenUnit childTokenUnit)
        {
            childTokenUnit.ParentTokenProp = regexPropInfo;
            childTokenUnit.ParentToken = this;
            childTokenUnit.Path = this.Path.Dot(regexPropInfo.Name); // update child name
        }
    }

    /// <summary>
    /// Only intended to be called by TokenTypeRegistry once upon startup. May be overridden by
    /// inheriting abstract classes who want to specify their own validation requirements.
    /// </summary>
    public virtual string ValidateStructure()
    {
        if (string.IsNullOrEmpty(Template.RegexString))
            return $"{nameof(Template.RegexString)} is null or empty";

        return null;
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

    public void PrependCardPathAllLevels(string cardName, int clauseIndex)
    {
        var prependValue = $"{cardName}-{clauseIndex}";
        GetChildTokens().ForEach(x => x.PrependCardPathAllLevels(cardName, clauseIndex));
        Path = $"{prependValue}-{Path}";
        IndexedPropertyCaptures.ForEach(x => x.Path = $"{prependValue}-{x.Path}");
    }


    //public override string ToString() => $"{Type.Name}{(MatchSpan.Source is null ? "" : $": {MatchSpan.ToStringValue()}")}";
    public override string ToString() => $"{Type.Name}{(Capture == null ? "" : $": {Capture.Value}")}";
}