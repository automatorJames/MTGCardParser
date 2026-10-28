namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fatespinner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fatespinner</exampledoc>
[TokenizationOrder(-1)]
public class FatespinnerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the player skips each instance of the chosen step or phase this turn"];
}
