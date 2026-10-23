namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ixidron, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ixidron</exampledoc>
[Dependent]
public class IxidronEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they're 2/2 creatures"];
}
