namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Flashback, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flashback</exampledoc>
[Dependent]
public class FlashbackCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast that card from your graveyard for its", Nib.This, "cost"];
}
