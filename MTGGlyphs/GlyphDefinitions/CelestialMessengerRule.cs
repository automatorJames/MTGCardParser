namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Celestial Messenger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Celestial Messenger</exampledoc>
[TokenizationOrder(-1)]
public class CelestialMessengerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flash (you may cast this card any time you could cast an instant"];
}
