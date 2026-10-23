namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Magmatic Galleon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Magmatic Galleon</exampledoc>
[Dependent]
public class MagmaticGalleonEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a treasure token"];
}
