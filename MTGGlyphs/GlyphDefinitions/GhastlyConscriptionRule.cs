namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ghastly Conscription, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ghastly Conscription</exampledoc>
[TokenizationOrder(-1)]
public class GhastlyConscriptionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all creature cards from target player's graveyard in a face-down pile, shuffle that pile, then manifest those cards"];
}
