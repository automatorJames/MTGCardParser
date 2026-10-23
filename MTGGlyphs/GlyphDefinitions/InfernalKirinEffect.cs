namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Infernal Kirin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Infernal Kirin</exampledoc>
[Dependent]
public class InfernalKirinEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player reveals their hand and discards all cards with that spell's mana value"];
}
