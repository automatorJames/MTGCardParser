namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gleam of Authority, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gleam of Authority</exampledoc>
[TokenizationOrder(-1)]
public class GleamOfAuthorityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature gets +1/+1 for each +1/+1 counter on other creatures you control"];
}
