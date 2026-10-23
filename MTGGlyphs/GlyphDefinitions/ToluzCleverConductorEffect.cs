namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Toluz, Clever Conductor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Toluz, Clever Conductor</exampledoc>
[Dependent]
public class ToluzCleverConductorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile them from your graveyard"];
}
