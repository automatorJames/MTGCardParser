namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Garruk, Curse Breaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Garruk, Curse Breaker</exampledoc>
/// <examplecapture>until your next turn, whenever one or more creatures attack one of your opponents, those creatures get +2/+2 and gain trample until end of turn</examplecapture>
[Dependent]
public class GarrukCurseBreakerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next turn, whenever one or more creatures attack one of your opponents, those creatures get +2/+2 and gain trample until end of turn"];
}
