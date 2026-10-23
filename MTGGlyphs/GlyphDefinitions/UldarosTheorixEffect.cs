namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Uldaros Theorix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Uldaros Theorix</exampledoc>
[Dependent]
public class UldarosTheorixEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you cast him, exile up to one target nonland card of each card type from your graveyard"];
}
