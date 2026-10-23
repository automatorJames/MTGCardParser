namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rhystic Scrying, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rhystic Scrying</exampledoc>
[TokenizationOrder(-1)]
public class RhysticScryingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if any player pays {2}, discard three cards"];
}
