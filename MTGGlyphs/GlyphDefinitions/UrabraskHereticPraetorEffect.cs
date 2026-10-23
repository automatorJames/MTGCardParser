namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Urabrask, Heretic Praetor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Urabrask, Heretic Praetor</exampledoc>
/// <examplecapture>the next time they would draw a card this turn, instead they exile the top card of their library</examplecapture>
[Dependent]
public class UrabraskHereticPraetorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time they would draw a card this turn, instead they exile the top card of their library"];
}
