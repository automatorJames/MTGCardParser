namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jackal, Genius Geneticist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jackal, Genius Geneticist</exampledoc>
[Dependent]
public class JackalGeniusGeneticistEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy that spell, except the copy isn't legendary"];
}
