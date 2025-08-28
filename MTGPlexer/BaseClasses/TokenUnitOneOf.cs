namespace MTGPlexer.BaseClasses;

public abstract class TokenUnitOneOf: TokenUnit
{
    List<PropertyInfo> _tokenUnitChildren;

    protected TokenUnitOneOf(params string[] templateSnippets) : base(templateSnippets)
    {
        _tokenUnitChildren = GetType().GetProps().ToList();
    }

    /// <summary>
    /// Returns a single non-null child TokenUnit which represents the "one" property with a value among 
    /// this instance's canddiate values. As an analytical precaution, an exception is thrown if no
    /// non-null TokenUnit is found.
    /// </summary>
    public TokenUnit GetSingleNonNullChildToken()
    {
        foreach (var prop in GetType().GetProps())
        {
            var propVal = prop.GetValue(this);

            if (propVal is TokenUnit tokenUnit)
                return tokenUnit;
        }

        throw new Exception("Expected a non-null TokenUnit child property, but found none");
    }

    public override bool ValidateStructure()
    {
        // There should be only TokenUnit props, and more than one

        if (_tokenUnitChildren.Any(x => !x.PropertyType.IsAssignableTo(typeof(TokenUnit))))
            return false;

        if (_tokenUnitChildren.Count() < 2) 
            return false;

        return true;
    }

    public static string GetTokenUnitOneOfRegexHeaderComment(Type tokenUnitOneOfType)
    {
        var tokenUnitChildPropNames = tokenUnitOneOfType.GetProps().Select(x => x.Name);
        return $"(?# {tokenUnitOneOfType.Name}: {string.Join(" | ", tokenUnitChildPropNames)})";
    }
}