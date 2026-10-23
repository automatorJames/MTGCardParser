namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Case of the Burning Masks, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Case of the Burning Masks</exampledoc>
[TokenizationOrder(-1)]
public class CaseOfTheBurningMasksRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["to solve — three or more sources you controlled dealt damage this turn"];
}
