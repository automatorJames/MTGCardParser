namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bound by Moonsilver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bound by Moonsilver</exampledoc>
[TokenizationOrder(-1)]
public class BoundByMoonsilverRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature can't attack, block, or transform"];
}
