namespace MTGPlexer.TokenUnitGraphComponents;

public abstract class TokenUnit
{
    public HydratedNodeGraph NodeGraph { get; set; }
    protected virtual Snippet[] Snippets { get; } = [];
    public Snippet[] GetSnippets() => Snippets;

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

    protected virtual void OnAfterHydrated()
    {
        // Base implementation requires no actions post-hydration
    }

    /// <summary>
    /// Only intended to be called by TokenTypeRegistry once upon startup. May be overridden by
    /// inheriting abstract classes who want to specify their own validation requirements.
    /// </summary>
    public virtual string ValidateStructure()
    {
        var rootNode = TokenTypeRegistry.RootNodes[Type];

        if (string.IsNullOrEmpty(rootNode.BuiltRegex.MinifiedRegexString))
            return $"{nameof(RootNode.BuiltRegex.MinifiedRegexString)} is null or empty";

        var expectedProps = Type.GetPublicPropNames();
        var missingProps = expectedProps.Except(rootNode.CaptureChildren.Select(x => x.PropertySnippet.Prop.Name)).ToList();
    
        if (missingProps.Any())
            return $"the following properties are not represented among template snippets: {string.Join(", ", missingProps)}";

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
}