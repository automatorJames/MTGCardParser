namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Stalwart Successor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stalwart Successor</exampledoc>
/// <examplecapture>if it's the first time counters have been put on that creature this turn, put a +1/+1 counter on that creature</examplecapture>
[Dependent]
public class StalwartSuccessorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's the first time counters have been put on that creature this turn, put a +1/+1 counter on that creature"];
}
