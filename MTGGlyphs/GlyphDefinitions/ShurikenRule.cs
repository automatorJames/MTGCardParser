namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shuriken, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shuriken</exampledoc>
[TokenizationOrder(-1)]
public class ShurikenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature's controller gains control of", Nib.This, "unless it was unattached from a ninja"];
}
