namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bone Dancer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bone Dancer</exampledoc>
[Dependent]
public class BoneDancerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put the top creature card of defending player's graveyard onto the battlefield under your control"];
}
