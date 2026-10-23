namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Silvan Reveler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Silvan Reveler</exampledoc>
[TokenizationOrder(-1)]
public class SilvanRevelerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you discard a land card this way, put it from your graveyard onto the battlefield tapped"];
}
