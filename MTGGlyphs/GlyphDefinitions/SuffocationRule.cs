namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Suffocation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Suffocation</exampledoc>
[TokenizationOrder(-1)]
public class SuffocationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 4 damage to the controller of the last red instant or sorcery spell that dealt damage to you this turn"];
}
