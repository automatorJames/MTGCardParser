namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Elemental Resonance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elemental Resonance</exampledoc>
[Dependent]
public class ElementalResonanceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add mana equal to enchanted permanent's mana cost"];
}
