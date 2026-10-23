namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mogg Assassin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mogg Assassin</exampledoc>
[TokenizationOrder(-1)]
public class MoggAssassinRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lose the flip, destroy the creature your opponent chose"];
}
