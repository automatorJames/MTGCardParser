namespace MTGPlexer.RegexGeneration.GraphNodes;

/// <summary>
/// Represents a placeholder text property of type PlaceholderCapture. This property type will typically have
/// a RegexPattern attribute defining its pattern, but in the absence of one the normalized property name will
/// be used as a pattern instead. This record is tightly coupled with the PlaceholderCapture type, which represents
/// a capture that's a placeholder in the sense that the caller wants to capture the given pattern but doesn't 
/// know how to decompose it yet, or the containing TokenUnit overrides SetPropertiesFromMatch and needs a property
/// to store an interim text value.
/// </summary>
public class PlaceholderNode : LeafNode
{
    public PlaceholderNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this, spaceDisposition: SpaceDisposition.DisallowedGlobal);
        builder.AddAlternateValues(ScalarAlternateSet.Alternates);
        builder.CloseGroup();
    }

    public override object GetValueSingle(Capture capture) =>
        new PlaceholderCapture(capture.Value);
}