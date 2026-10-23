namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Zimone, All-Questioning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zimone, All-Questioning</exampledoc>
/// <examplecapture>if a land entered the battlefield under your control this turn and you control a prime number of lands, create primo, the indivisible, a legendary 0/0 green and blue fractal creature token, then put that many +1/+1 counters on it</examplecapture>
[Dependent]
public class ZimoneAllQuestioningEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a land entered the battlefield under your control this turn and you control a prime number of lands, create primo, the indivisible, a legendary 0/0 green and blue fractal creature token, then put that many +1/+1 counters on it"];
}
