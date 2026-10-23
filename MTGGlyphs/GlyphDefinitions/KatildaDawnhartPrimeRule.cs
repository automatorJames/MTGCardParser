namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Katilda, Dawnhart Prime, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Katilda, Dawnhart Prime</exampledoc>
[TokenizationOrder(-1)]
public class KatildaDawnhartPrimeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["human creatures you control have \"{t}: add one mana of any of this creature's colors"];
}
