namespace MTGPlexer.BaseClasses;

public abstract class TokenUnitOneOf: TokenUnit
{
    protected TokenUnitOneOf(params string[] templateSnippets) : base(templateSnippets) { }

    public override bool ValidateStructure()
    {
        // There should be only TokenUnit props, and there should be more than 1

        var props = GetType().GetPublicDeclaredProps();

        if (props.Any(x => !x.PropertyType.IsAssignableTo(typeof(TokenUnit))))
            return false;

        if (props.Count() < 2) 
            return false;

        return true;
    }
}

