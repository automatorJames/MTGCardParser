namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Genesis Wave, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Genesis Wave</exampledoc>
[TokenizationOrder(-1)]
public class GenesisWaveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then put all cards revealed this way that weren't put onto the battlefield into your graveyard"];
}
