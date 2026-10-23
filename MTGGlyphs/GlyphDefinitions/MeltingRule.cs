namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Melting, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Melting</exampledoc>
[TokenizationOrder(-1)]
public class MeltingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all lands are no longer snow"];
}
