namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of End Hostilities, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>End Hostilities</exampledoc>
[TokenizationOrder(-1)]
public class EndHostilitiesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures and all permanents attached to creatures"];
}
