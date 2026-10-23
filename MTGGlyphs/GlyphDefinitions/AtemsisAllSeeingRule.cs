namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Atemsis, All-Seeing, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Atemsis, All-Seeing</exampledoc>
[TokenizationOrder(-1)]
public class AtemsisAllSeeingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if cards with at least six different mana values are revealed this way, that player loses the game"];
}
