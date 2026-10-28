namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jackal, Genius Geneticist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jackal, Genius Geneticist</exampledoc>
[TokenizationOrder(-1)]
public class JackalGeniusGeneticistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast a creature spell with mana value equal to jackal's power, copy that spell, except the copy isn't legendary"];
}
