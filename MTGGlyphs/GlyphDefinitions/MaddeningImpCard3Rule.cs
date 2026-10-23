namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Maddening Imp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Maddening Imp</exampledoc>
[TokenizationOrder(-1)]
public class MaddeningImpCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only during an opponent's turn and only before combat"];
}
