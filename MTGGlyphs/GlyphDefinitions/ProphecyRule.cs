namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Prophecy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Prophecy</exampledoc>
[TokenizationOrder(-1)]
public class ProphecyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reveal the top card of target opponent's library"];
}
