namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Temporary Insanity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Temporary Insanity</exampledoc>
[TokenizationOrder(-1)]
public class TemporaryInsanityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap target creature with power less than the number of cards in your graveyard and gain control of it until end of turn"];
}
