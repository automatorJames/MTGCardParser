namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Brilliance Unleashed, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brilliance Unleashed</exampledoc>
[TokenizationOrder(-1)]
public class BrillianceUnleashedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, return it to the battlefield and it's a 3/3 robot artifact creature with flying"];
}
