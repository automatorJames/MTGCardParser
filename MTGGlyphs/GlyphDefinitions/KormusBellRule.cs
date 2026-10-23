namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kormus Bell, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kormus Bell</exampledoc>
[TokenizationOrder(-1)]
public class KormusBellRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all swamps are 1/1 black creatures that are still lands"];
}
