namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Desperate Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Desperate Gambit</exampledoc>
[TokenizationOrder(-1)]
public class DesperateGambitCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a source you control and flip a coin"];
}
