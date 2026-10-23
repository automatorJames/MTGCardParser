namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Everlasting Torment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Everlasting Torment</exampledoc>
[Dependent]
public class EverlastingTormentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a source with wither deals damage to creatures in the form of -1/-1 counters"];
}
