namespace MTGCardParser;

public record TokenUnitPropertyCapture(PropertyInfo Prop) : PropertyCapture(Prop)
{
    private string _lazyRegexPattern;

    public override string RegexPattern
    {
        get => _lazyRegexPattern ??= GetTokenRegexPattern(Prop);
    }

    private static string GetTokenRegexPattern(PropertyInfo prop)
    {
        var childType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        if (!TokenClassRegistry.TypeRegexTemplates.TryGetValue(childType, out var template))
        {
            throw new InvalidOperationException($"Could not find registered RegexTemplate for child type '{childType.Name}'.");
        }
        // CORRECTED: Make the entire group optional
        return $"(?<{prop.Name}>{template.RenderedRegexString})?";
    }

    public override void HydrateProperty(TokenUnit instance, Match match)
    {
        var group = match.Groups[CaptureGroupName];
        if (!group.Success) return;

        var subSpan = GetSubSpanFromGroup(instance.MatchSpan, group)!.Value;
        var childInstance = TokenUnit.InstantiateFromMatchString(Prop.PropertyType, subSpan, instance);

        Prop.SetValue(instance, childInstance);
        instance.ChildTokens.Add(childInstance);
        instance.PropMatches[this] = subSpan;
    }
}