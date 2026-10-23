namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Equinox, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Equinox</exampledoc>
[TokenizationOrder(-1)]
public class EquinoxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted land has \"{t}: counter target spell if it would destroy a land you control"];
}
