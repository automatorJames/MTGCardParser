namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Investigator's Journal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Investigator's Journal</exampledoc>
[TokenizationOrder(-1)]
public class InvestigatorsJournalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this artifact enters with a number of suspect counters on it equal to the greatest number of creatures a player controls"];
}
