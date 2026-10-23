namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ravager Wurm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ravager Wurm</exampledoc>
[TokenizationOrder(-1)]
public class RavagerWurmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• destroy target land with an activated ability that isn't a mana ability"];
}
