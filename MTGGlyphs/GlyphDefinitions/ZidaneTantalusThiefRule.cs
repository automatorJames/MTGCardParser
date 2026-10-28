namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Zidane, Tantalus Thief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zidane, Tantalus Thief</exampledoc>
[TokenizationOrder(-1)]
public class ZidaneTantalusThiefRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an opponent gains control of a permanent from you, you create a treasure token"];
}
