namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Corrupted Grafstone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Corrupted Grafstone</exampledoc>
[Dependent]
public class CorruptedGrafstoneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a color of a card in your graveyard"];
}
