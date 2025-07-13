namespace MTGPlexer.BaseClasses;

public abstract class TokenUnitComplex : TokenUnit
{
    public abstract void SetComplexValuesFromMatch();

    public override void SetPropertiesFromMatch()
    {
        // First, allow the base class to set all properties normally
        base.SetPropertiesFromMatch();

        // Second, apply whatever class-specific decomposition is necessary
        SetComplexValuesFromMatch();
    }

    protected TokenUnitComplex(params string[] templateSnippets) : base(templateSnippets) { }
}

