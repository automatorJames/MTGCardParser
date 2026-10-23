namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Knight of Valor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Knight of Valor</exampledoc>
[Dependent]
public class KnightOfValorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature without flanking blocking this creature gets -1/-1 until end of turn"];
}
