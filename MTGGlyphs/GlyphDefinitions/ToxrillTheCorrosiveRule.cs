namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Toxrill, the Corrosive, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Toxrill, the Corrosive</exampledoc>
[TokenizationOrder(-1)]
public class ToxrillTheCorrosiveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures you don't control get -1/-1 for each slime counter on them"];
}
