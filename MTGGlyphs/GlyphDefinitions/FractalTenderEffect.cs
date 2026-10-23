namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fractal Tender, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fractal Tender</exampledoc>
/// <examplecapture>if you put a counter on this creature this turn, create a 0/0 green and blue fractal creature token and put three +1/+1 counters on it</examplecapture>
[Dependent]
public class FractalTenderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you put a counter on this creature this turn, create a 0/0 green and blue fractal creature token and put three +1/+1 counters on it"];
}
