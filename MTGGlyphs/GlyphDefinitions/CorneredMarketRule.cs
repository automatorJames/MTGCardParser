namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cornered Market, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cornered Market</exampledoc>
[TokenizationOrder(-1)]
public class CorneredMarketRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players can't play nonbasic lands with the same name as a nontoken permanent"];
}
