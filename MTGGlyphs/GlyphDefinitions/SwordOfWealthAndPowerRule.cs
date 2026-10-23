namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sword of Wealth and Power, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sword of Wealth and Power</exampledoc>
[TokenizationOrder(-1)]
public class SwordOfWealthAndPowerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature gets +2/+2 and has protection from instants and from sorceries"];
}
