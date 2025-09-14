using MTGPlexer.CommonDTOs.StructuredMatches;

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
    public StructuredMatchBase TokenMatch { get; set; }

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

    public List<TokenUnit> GetChildTokens() => IndexedPropertyCaptures
        .Where(x => x.IsChildToken)
        .Select(x => x.Value)
        .OfType<TokenUnit>()
        .ToList();

    public void SetPropertyFromMatch(RegexPropInfo regexPropInfo, StructuredMatchBase match, object propVal)
    {
        regexPropInfo.Prop.SetValue(this, propVal);
        var capturePosition = IndexedPropertyCaptures.Count;
        IndexedPropertyCaptures.Add(new(regexPropInfo, match, propVal, capturePosition));

        if (propVal is TokenUnit childTokenUnit)
        {
            childTokenUnit.ParentTokenProp = regexPropInfo;
            childTokenUnit.ParentToken = this;
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


    //public override string ToString() => $"{Type.Name}{(MatchSpan.Source is null ? "" : $": {MatchSpan.ToStringValue()}")}";
    public override string ToString() => $"{Type.Name}{(TokenMatch == null ? "" : $": {TokenMatch.Value}")}";
}