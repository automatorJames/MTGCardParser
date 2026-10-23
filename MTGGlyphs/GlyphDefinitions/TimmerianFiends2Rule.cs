namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Timmerian Fiends, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Timmerian Fiends</exampledoc>
[TokenizationOrder(-1)]
public class TimmerianFiends2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put the artifact card into your graveyard and", Nib.This, "from anywhere into that player's graveyard"];
}
