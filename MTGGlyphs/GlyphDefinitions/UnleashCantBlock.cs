namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Unleash's reminder text, second sentence.</summary>
/// <exampledoc>Bloodfray Giant</exampledoc>
/// <examplecapture>it can't block as long as it has a +1/+1 counter on it</examplecapture>
[TokenizationOrder(0)]
public class UnleashCantBlock : Glyph
{
    public override Nib[] Nibs => ["it can't block as long as it has a +1/+1 counter on it"];
}
