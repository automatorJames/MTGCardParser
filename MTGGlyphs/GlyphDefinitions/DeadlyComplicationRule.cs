namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Deadly Complication, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Deadly Complication</exampledoc>
[TokenizationOrder(-1)]
public class DeadlyComplicationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have it become no longer suspected"];
}
