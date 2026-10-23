namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rally the Righteous, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rally the Righteous</exampledoc>
[TokenizationOrder(-1)]
public class RallyTheRighteousRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["radiance — untap target creature and each other creature that shares a color with it"];
}
