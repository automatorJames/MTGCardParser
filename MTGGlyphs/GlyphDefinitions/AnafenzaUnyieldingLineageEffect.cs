namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Anafenza, Unyielding Lineage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Anafenza, Unyielding Lineage</exampledoc>
[Dependent]
public class AnafenzaUnyieldingLineageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["anafenza endures 2"];
}
