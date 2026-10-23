namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Winnowing, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winnowing</exampledoc>
[TokenizationOrder(-1)]
public class WinnowingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then each player sacrifices all other creatures they control that don't share a creature type with the chosen creature they control"];
}
