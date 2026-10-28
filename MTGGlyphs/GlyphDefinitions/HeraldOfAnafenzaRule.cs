namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Herald of Anafenza, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Herald of Anafenza</exampledoc>
[TokenizationOrder(-1)]
public class HeraldOfAnafenzaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you activate this creature's outlast ability, create a 1/1 white warrior creature token"];
}
