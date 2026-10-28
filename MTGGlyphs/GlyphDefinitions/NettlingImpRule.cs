namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nettling Imp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nettling Imp</exampledoc>
[TokenizationOrder(-1)]
public class NettlingImpRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only during an opponent's turn, before attackers are declared"];
}
