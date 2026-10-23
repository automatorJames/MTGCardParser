namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lantern Flare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lantern Flare</exampledoc>
[TokenizationOrder(-1)]
public class LanternFlareRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["[x is the number of creatures you control"];
}
