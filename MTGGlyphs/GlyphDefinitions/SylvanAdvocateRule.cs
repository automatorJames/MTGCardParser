namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sylvan Advocate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sylvan Advocate</exampledoc>
[TokenizationOrder(-1)]
public class SylvanAdvocateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as you control six or more lands, this creature and land creatures you control get +2/+2"];
}
