namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of All Hallow's Eve, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>All Hallow's Eve</exampledoc>
/// <examplecapture>if there are no more scream counters on it, put it into your graveyard and each player returns all creature cards from their graveyard to the battlefield</examplecapture>
[TokenizationOrder(-1)]
public class AllHallowsEveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if there are no more scream counters on it, put it into your graveyard and each player returns all creature cards from their graveyard to the battlefield"];
}
