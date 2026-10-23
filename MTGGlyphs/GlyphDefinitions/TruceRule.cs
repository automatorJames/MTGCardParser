namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Truce, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Truce</exampledoc>
[TokenizationOrder(-1)]
public class TruceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each card less than two a player draws this way, that player gains 2 life"];
}
