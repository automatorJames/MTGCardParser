namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Loading Zone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Loading Zone</exampledoc>
/// <examplecapture>if one or more counters would be put on a creature, spacecraft, or planet you control, twice that many of each of those kinds of counters are put on it instead</examplecapture>
[TokenizationOrder(-1)]
public class LoadingZoneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if one or more counters would be put on a creature, spacecraft, or planet you control, twice that many of each of those kinds of counters are put on it instead"];
}
