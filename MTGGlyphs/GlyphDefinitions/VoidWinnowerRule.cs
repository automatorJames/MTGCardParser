namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Void Winnower, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Void Winnower</exampledoc>
[TokenizationOrder(-1)]
public class VoidWinnowerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["your opponents can't block with creatures with even mana values"];
}
