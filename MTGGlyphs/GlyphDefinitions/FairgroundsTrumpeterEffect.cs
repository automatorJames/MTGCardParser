namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fairgrounds Trumpeter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fairgrounds Trumpeter</exampledoc>
/// <examplecapture>if a +1/+1 counter was put on a permanent under your control this turn, put a +1/+1 counter on this creature</examplecapture>
[Dependent]
public class FairgroundsTrumpeterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a +1/+1 counter was put on a permanent under your control this turn, put a +1/+1 counter on this creature"];
}
