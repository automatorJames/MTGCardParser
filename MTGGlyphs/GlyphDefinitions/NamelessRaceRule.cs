namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nameless Race, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nameless Race</exampledoc>
/// <examplecapture>the amount you pay can't be more than the total number of white nontoken permanents your opponents control plus the total number of white cards in their graveyards</examplecapture>
[TokenizationOrder(-1)]
public class NamelessRaceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the amount you pay can't be more than the total number of white nontoken permanents your opponents control plus the total number of white cards in their graveyards"];
}
