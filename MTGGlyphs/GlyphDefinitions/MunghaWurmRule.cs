namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mungha Wurm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mungha Wurm</exampledoc>
[TokenizationOrder(-1)]
public class MunghaWurmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't untap more than one land during your untap step"];
}
