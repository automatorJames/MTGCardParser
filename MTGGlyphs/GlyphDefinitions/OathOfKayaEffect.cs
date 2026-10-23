namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Oath of Kaya, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oath of Kaya</exampledoc>
[Dependent]
public class OathOfKayaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 2 damage to that player and you gain 2 life"];
}
