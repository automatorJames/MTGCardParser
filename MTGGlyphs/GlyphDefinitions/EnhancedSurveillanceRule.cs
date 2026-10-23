namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Enhanced Surveillance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Enhanced Surveillance</exampledoc>
[TokenizationOrder(-1)]
public class EnhancedSurveillanceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may look at an additional two cards each time you surveil"];
}
