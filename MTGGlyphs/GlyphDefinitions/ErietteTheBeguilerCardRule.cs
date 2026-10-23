namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Eriette, the Beguiler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eriette, the Beguiler</exampledoc>
[TokenizationOrder(-1)]
public class ErietteTheBeguilerCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an aura you control becomes attached to a nonland permanent an opponent controls with mana value less than or equal to that aura's mana value, gain control of that permanent for as long as that aura is attached to it"];
}
