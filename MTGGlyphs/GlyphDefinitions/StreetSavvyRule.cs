namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Street Savvy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Street Savvy</exampledoc>
[TokenizationOrder(-1)]
public class StreetSavvyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature gets +0/+2 and can block creatures with landwalk abilities as though they didn't have those abilities"];
}
