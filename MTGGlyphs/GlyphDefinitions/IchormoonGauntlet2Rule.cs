namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ichormoon Gauntlet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ichormoon Gauntlet</exampledoc>
[TokenizationOrder(-1)]
public class IchormoonGauntlet2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put an additional counter of that kind on that permanent"];
}
