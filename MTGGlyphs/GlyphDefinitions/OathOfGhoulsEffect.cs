namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Oath of Ghouls, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oath of Ghouls</exampledoc>
/// <examplecapture>that player chooses target player whose graveyard has fewer creature cards in it than their graveyard does and is their opponent</examplecapture>
[Dependent]
public class OathOfGhoulsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player chooses target player whose graveyard has fewer creature cards in it than their graveyard does and is their opponent"];
}
