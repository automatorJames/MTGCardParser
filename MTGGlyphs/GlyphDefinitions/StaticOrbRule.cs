namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Static Orb, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Static Orb</exampledoc>
[TokenizationOrder(-1)]
public class StaticOrbRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as this artifact is untapped, players can't untap more than two permanents during their untap steps"];
}
