namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Emrakul, the Promised End, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Emrakul, the Promised End</exampledoc>
[TokenizationOrder(-1)]
public class EmrakulThePromisedEndRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["after that turn, that player takes an extra turn"];
}
