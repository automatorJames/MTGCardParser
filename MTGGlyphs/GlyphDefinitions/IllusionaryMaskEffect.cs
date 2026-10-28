namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Illusionary Mask, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illusionary Mask</exampledoc>
/// <examplecapture>you may choose a creature card in your hand whose mana cost could be paid by some amount of, or all of, the mana you spent on {x}</examplecapture>
[Dependent]
public class IllusionaryMaskEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may choose a creature card in your hand whose mana cost could be paid by some amount of, or all of, the mana you spent on {x}"];
}
