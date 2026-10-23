namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Turnabout, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Turnabout</exampledoc>
[TokenizationOrder(-1)]
public class TurnaboutRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap all untapped permanents of the chosen type target player controls, or untap all tapped permanents of that type that player controls"];
}
