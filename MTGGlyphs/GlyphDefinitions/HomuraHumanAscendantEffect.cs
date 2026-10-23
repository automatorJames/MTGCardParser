namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Homura, Human Ascendant // Homura's Essence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Homura, Human Ascendant // Homura's Essence</exampledoc>
[Dependent]
public class HomuraHumanAscendantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return it to the battlefield flipped"];
}
