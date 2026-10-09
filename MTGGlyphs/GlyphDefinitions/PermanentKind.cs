namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What kind of permanent something is: a card type ("land") or a creature type ("wall").</summary>
/// <exampledoc>Ice Storm</exampledoc>
/// <examplecapture>land</examplecapture>
[Dependent]
public class PermanentKind : GlyphOneOf
{
    public CardType? CardType { get; set; }
    public CreatureType? CreatureType { get; set; }
}
