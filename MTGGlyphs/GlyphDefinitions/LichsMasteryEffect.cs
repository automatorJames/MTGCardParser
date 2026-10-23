namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lich's Mastery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lich's Mastery</exampledoc>
[Dependent]
public class LichsMasteryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each 1 life you lost, exile a permanent you control or a card from your hand or graveyard"];
}
