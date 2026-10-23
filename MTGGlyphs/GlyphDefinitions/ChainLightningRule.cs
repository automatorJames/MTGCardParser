namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chain Lightning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chain Lightning</exampledoc>
[TokenizationOrder(-1)]
public class ChainLightningRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then that player or that permanent's controller may pay {r}{r}"];
}
