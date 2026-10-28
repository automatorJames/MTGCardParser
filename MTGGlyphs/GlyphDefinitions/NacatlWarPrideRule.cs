namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nacatl War-Pride, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nacatl War-Pride</exampledoc>
[TokenizationOrder(-1)]
public class NacatlWarPrideRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature must be blocked by exactly one creature if able"];
}
