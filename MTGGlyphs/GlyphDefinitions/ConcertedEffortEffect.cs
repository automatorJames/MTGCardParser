namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Concerted Effort, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Concerted Effort</exampledoc>
[Dependent]
public class ConcertedEffortEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures you control gain flying until end of turn if a creature you control has flying"];
}
