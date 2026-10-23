namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chandra's Incinerator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra's Incinerator</exampledoc>
[TokenizationOrder(-1)]
public class ChandrasIncinerator2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this spell costs {x} less to cast, where x is the total amount of noncombat damage dealt to your opponents this turn"];
}
