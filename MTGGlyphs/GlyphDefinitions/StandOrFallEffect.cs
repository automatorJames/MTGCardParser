namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Stand or Fall, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stand or Fall</exampledoc>
/// <examplecapture>for each defending player, separate all creatures that player controls into two piles and that player chooses one</examplecapture>
[Dependent]
public class StandOrFallEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each defending player, separate all creatures that player controls into two piles and that player chooses one"];
}
