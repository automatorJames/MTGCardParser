namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Zidane, Tantalus Thief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zidane, Tantalus Thief</exampledoc>
[Dependent]
public class ZidaneTantalusThiefEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you create a treasure token"];
}
