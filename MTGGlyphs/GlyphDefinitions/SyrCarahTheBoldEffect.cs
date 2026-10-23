namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Syr Carah, the Bold, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Syr Carah, the Bold</exampledoc>
[Dependent]
public class SyrCarahTheBoldEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile the top card of your library"];
}
