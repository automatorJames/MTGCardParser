namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tezzeret, Cruel Machinist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tezzeret, Cruel Machinist</exampledoc>
[TokenizationOrder(-1)]
public class TezzeretCruelMachinistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they're 5/5 artifact creatures"];
}
