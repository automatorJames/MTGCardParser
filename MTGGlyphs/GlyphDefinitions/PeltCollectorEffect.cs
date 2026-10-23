namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pelt Collector, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pelt Collector</exampledoc>
[Dependent]
public class PeltCollectorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that creature's power is greater than this creature's, put a +1/+1 counter on this creature"];
}
