namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Blaze of Glory, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blaze of Glory</exampledoc>
[TokenizationOrder(-1)]
public class BlazeOfGloryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it blocks each attacking creature this turn if able"];
}
