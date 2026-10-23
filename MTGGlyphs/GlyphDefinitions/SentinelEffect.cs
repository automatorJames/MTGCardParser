namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sentinel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sentinel</exampledoc>
[Dependent]
public class SentinelEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature's base toughness becomes equal to 1 plus the power of target creature blocking or blocked by this creature"];
}
