namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Preacher of the Schism, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Preacher of the Schism</exampledoc>
[TokenizationOrder(-1)]
public class PreacherOfTheSchismCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature attacks the player with the most life or tied for most life, create a 1/1 white vampire creature token with lifelink"];
}
