namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hunting Wilds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hunting Wilds</exampledoc>
[TokenizationOrder(-1)]
public class HuntingWildsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was kicked, untap all forests put onto the battlefield this way"];
}
