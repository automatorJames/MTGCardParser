namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Replenish, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Replenish</exampledoc>
[Dependent]
public class ReplenishEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["auras with nothing to enchant remain in your graveyard"];
}
