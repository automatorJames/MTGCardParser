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

    public TokenUnit ParentToken { get; set; }
    public RegexPropInfo ParentTokenProp { get; set; }
    public int RecursiveDepth { get; set; }
    //public TextSpan MatchSpan { get; set; }
    public StructuredMatch TokenMatch { get; set; }

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
    }

    //public static TokenUnit InstantiateFromMatchString(Type type, TextSpan matchSpan, TokenUnit parentToken = null, RegexPropInfo parentTokenProp = null)
    //{
    //    if (!type.IsAssignableTo(typeof(TokenUnit)))
    //        throw new Exception($"{type.Name} does not implement {nameof(TokenUnit)}");
    //
    //    var tokenInstance = (TokenUnit)Activator.CreateInstance(type);
    //    tokenInstance.ParentToken = parentToken;
    //    tokenInstance.ParentTokenProp = parentTokenProp;
    //    tokenInstance.MatchSpan = matchSpan;
    //    tokenInstance.SetPropertiesFromMatch();
    //
    //    tokenInstance.RecursiveDepth = 
    //        parentToken is null ? 0 
    //        : parentToken is TokenUnitOneOf ? parentToken.RecursiveDepth
    //        : parentToken.RecursiveDepth + 1;
    //
    //    return tokenInstance;
    //}

    public List<TokenUnit> GetChildTokens() => IndexedPropertyCaptures
        .Where(x => x.IsChildToken)
        .Select(x => x.Value)
        .OfType<TokenUnit>()
        .ToList();

    //public virtual void SetPropertiesFromMatch()
    //{
    //    Template.CaptureGroupProps.ForEach(x => x.SetValueFromMatchSpan(this, MatchSpan));
    //}

    //public void SetPropertyCapture(RegexPropInfo regexPropInfo, TextSpan textSpan, object propVal)
    //{
    //    regexPropInfo.Prop.SetValue(this, propVal);
    //    var capturePosition = IndexedPropertyCaptures.Count;
    //    IndexedPropertyCaptures.Add(new(regexPropInfo, textSpan, propVal, capturePosition));
    //}

    public void SetPropertyFromMatch(RegexPropInfo regexPropInfo, StructuredMatch match, object propVal)
    {
        regexPropInfo.Prop.SetValue(this, propVal);
        var capturePosition = IndexedPropertyCaptures.Count;
        IndexedPropertyCaptures.Add(new(regexPropInfo, match, propVal, capturePosition));
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


    //public override string ToString() => $"{Type.Name}{(MatchSpan.Source is null ? "" : $": {MatchSpan.ToStringValue()}")}";
    public override string ToString() => $"{Type.Name}{(TokenMatch == null ? "" : $": {TokenMatch.Value}")}";
}