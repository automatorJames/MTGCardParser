namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Faerie Slumber Party, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Faerie Slumber Party</exampledoc>
/// <examplecapture>for each opponent who controlled a creature returned this way, you create two 1/1 blue faerie creature tokens with flying and "this token can block only creatures with flying</examplecapture>
[TokenizationOrder(-1)]
public class FaerieSlumberPartyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each opponent who controlled a creature returned this way, you create two 1/1 blue faerie creature tokens with flying and \"this token can block only creatures with flying"];
}
