namespace MTGPlexer.GeneralDTOs;

public record StructuredMatch
{
    public Type Type { get; }
    public Match Match { get; }
    public string Value { get; }
    public string OriginalText { get; protected set; }
    public int Index { get; protected set; }
    public int Length { get; protected set; }
    public int End { get; protected set; }
    public StructuredMatch[] Ancestors { get; } = [];

    public StructuredMatch(Type type, Match match, StructuredMatch parentMatch = null) 
    {
        Type = type;
        Match = match;
        Value = match.Value;
        Ancestors = parentMatch == null ? [] : parentMatch.Ancestors.Concat([parentMatch]).ToArray();
        SetOriginalTextAndPosition();
    }

    protected virtual void SetOriginalTextAndPosition()
    {
        OriginalText = GetOriginalTextFromMatch(Match);
        Index = Match.Index;
        Length = Match.Length;
        End = Index + Length;
    }

    static string GetOriginalTextFromMatch(Match match)
    {
        if (match == null)
        {
            throw new ArgumentNullException(nameof(match));
        }

        // Get the PropertyInfo for the internal "Text" property.
        // We need to use BindingFlags.NonPublic and BindingFlags.Instance
        // to find internal/private instance properties.
        // Type.GetProperty() will search up the inheritance hierarchy.
        var propertyInfo = typeof(Match).GetProperty(
            "Text",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (propertyInfo == null)
        {
            // This could happen if a future .NET version renames or removes the property.
            throw new InvalidOperationException("The internal 'Text' property could not be found on the Match/Capture type.");
        }

        // Get the value of the property from the match instance.
        return propertyInfo.GetValue(match) as string;
    }

    public StructuredMatch GetChildMatch(CaptureGroupPropBase captureGroup)
    {
        var match = captureGroup.MatchRegex.Match(Value);

        if (!match.Success) 
            return null;

        StructuredMatch child = new(captureGroup.RegexPropInfo.UnderlyingType, match, this);

        return child;
    }

    public StructuredSubCapture GetChildSubCapture(CaptureGroupPropBase captureGroup, Capture subCapture)
    {
        var match = captureGroup.MatchRegex.Match(subCapture.Value);

        if (!match.Success)
            throw new Exception("When a subMatch is provided, it must be found in the captureGroup");

        StructuredSubCapture child = new(captureGroup.RegexPropInfo.UnderlyingType, match, this, subCapture);

        return child;
    }
}

