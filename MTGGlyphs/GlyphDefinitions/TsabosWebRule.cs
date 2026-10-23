namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tsabo's Web, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tsabo's Web</exampledoc>
[TokenizationOrder(-1)]
public class TsabosWebRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each land with an activated ability that isn't a mana ability doesn't untap during its controller's untap step"];
}
