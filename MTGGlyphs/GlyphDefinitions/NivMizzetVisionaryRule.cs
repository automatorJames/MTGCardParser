namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Niv-Mizzet, Visionary, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Niv-Mizzet, Visionary</exampledoc>
[TokenizationOrder(-1)]
public class NivMizzetVisionaryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a source you control deals noncombat damage to an opponent, you draw that many cards"];
}
