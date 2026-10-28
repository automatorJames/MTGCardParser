namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Poultice Sliver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Poultice Sliver</exampledoc>
[TokenizationOrder(-1)]
public class PoulticeSliverRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(the next time that sliver would be destroyed this turn, instead tap it, remove it from combat, and heal all damage on it"];
}
