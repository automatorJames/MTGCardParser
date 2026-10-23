namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Over the Top, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Over the Top</exampledoc>
/// <examplecapture>each player reveals a number of cards from the top of their library equal to the number of nonland permanents they control, puts all permanent cards they revealed this way onto the battlefield, and puts the rest into their graveyard</examplecapture>
[TokenizationOrder(-1)]
public class OverTheTopRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player reveals a number of cards from the top of their library equal to the number of nonland permanents they control, puts all permanent cards they revealed this way onto the battlefield, and puts the rest into their graveyard"];
}
