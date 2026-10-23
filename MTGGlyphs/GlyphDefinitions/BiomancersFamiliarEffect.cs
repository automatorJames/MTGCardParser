namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Biomancer's Familiar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Biomancer's Familiar</exampledoc>
[Dependent]
public class BiomancersFamiliarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time target creature adapts this turn, it adapts as though it had no +1/+1 counters on it"];
}
