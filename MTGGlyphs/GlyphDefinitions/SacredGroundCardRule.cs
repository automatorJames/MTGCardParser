namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sacred Ground, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sacred Ground</exampledoc>
[TokenizationOrder(-1)]
public class SacredGroundCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a spell or ability an opponent controls causes a land to be put into your graveyard from the battlefield, return that card to the battlefield"];
}
