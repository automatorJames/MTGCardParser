namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Death Spark, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Death Spark</exampledoc>
[Dependent]
public class DeathSparkEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this card is in your graveyard with a creature card directly above it, you may pay {1}"];
}
