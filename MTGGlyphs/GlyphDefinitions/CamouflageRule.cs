namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Camouflage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Camouflage</exampledoc>
/// <examplecapture>this turn, instead of declaring blockers, each defending player chooses any number of creatures they control and divides them into a number of piles equal to the number of attacking creatures for whom that player is the defending player</examplecapture>
[TokenizationOrder(-1)]
public class CamouflageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this turn, instead of declaring blockers, each defending player chooses any number of creatures they control and divides them into a number of piles equal to the number of attacking creatures for whom that player is the defending player"];
}
