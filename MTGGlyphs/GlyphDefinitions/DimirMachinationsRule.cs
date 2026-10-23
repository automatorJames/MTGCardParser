namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dimir Machinations, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dimir Machinations</exampledoc>
[TokenizationOrder(-1)]
public class DimirMachinationsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile any number of those cards, then put the rest back in any order"];
}
