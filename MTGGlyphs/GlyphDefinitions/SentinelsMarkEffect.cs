namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sentinel's Mark, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sentinel's Mark</exampledoc>
[Dependent]
public class SentinelsMarkEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you cast it during your main phase, enchanted creature gains lifelink until end of turn"];
}
