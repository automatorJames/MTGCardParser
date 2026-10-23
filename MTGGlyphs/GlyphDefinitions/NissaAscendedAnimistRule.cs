namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nissa, Ascended Animist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nissa, Ascended Animist</exampledoc>
[TokenizationOrder(-1)]
public class NissaAscendedAnimistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each {g/p} paid with life, this planeswalker enters with two fewer loyalty counters"];
}
