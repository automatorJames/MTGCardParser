namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Eshki Dragonclaw, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eshki Dragonclaw</exampledoc>
/// <examplecapture>if you've cast both a creature spell and a noncreature spell this turn, draw a card and put two +1/+1 counters on {this}</examplecapture>
[Dependent]
public class EshkiDragonclawEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you've cast both a creature spell and a noncreature spell this turn, draw a card and put two +1/+1 counters on", Nib.This];
}
