namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Serene Remembrance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Serene Remembrance</exampledoc>
[TokenizationOrder(-1)]
public class SereneRemembranceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle", Nib.This, "and up to three target cards from a single graveyard into their owners' libraries"];
}
