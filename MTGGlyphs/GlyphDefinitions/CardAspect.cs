namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a card can be, as in "isn't a creature": a card type or a color.</summary>
/// <exampledoc>Animate Artifact</exampledoc>
/// <examplecapture>creature</examplecapture>
[Dependent]
public class CardAspect : GlyphOneOf
{
    public CardType? CardType { get; set; }
    public ManaColor? ManaColor { get; set; }
}
