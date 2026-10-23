namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Airtight Alibi, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Airtight Alibi</exampledoc>
[TokenizationOrder(-1)]
public class AirtightAlibiRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature gets +2/+2 and can't become suspected"];
}
