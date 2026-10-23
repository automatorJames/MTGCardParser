namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Karai, Future of the Foot, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karai, Future of the Foot</exampledoc>
[TokenizationOrder(-1)]
public class KaraiFutureOfTheFootRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if her sneak cost was paid this turn, instead return that card to the battlefield"];
}
