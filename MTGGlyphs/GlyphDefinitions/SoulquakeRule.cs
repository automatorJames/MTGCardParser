namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Soulquake, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soulquake</exampledoc>
[TokenizationOrder(-1)]
public class SoulquakeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return all creatures on the battlefield and all creature cards in graveyards to their owners' hands"];
}
