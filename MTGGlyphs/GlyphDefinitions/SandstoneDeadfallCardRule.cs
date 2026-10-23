namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sandstone Deadfall, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sandstone Deadfall</exampledoc>
[TokenizationOrder(-1)]
public class SandstoneDeadfallCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{t}, sacrifice two lands and this artifact: destroy target attacking creature"];
}
