namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nahiri, Storm of Stone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nahiri, Storm of Stone</exampledoc>
[TokenizationOrder(-1)]
public class NahiriStormOfStoneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your turn, creatures you control have first strike and equip abilities you activate cost {1} less to activate"];
}
