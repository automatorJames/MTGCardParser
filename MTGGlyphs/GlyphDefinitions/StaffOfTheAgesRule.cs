namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Staff of the Ages, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Staff of the Ages</exampledoc>
[TokenizationOrder(-1)]
public class StaffOfTheAgesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures with landwalk abilities can be blocked as though they didn't have those abilities"];
}
