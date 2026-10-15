namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Evolve's reminder text.</summary>
/// <exampledoc>Adaptive Snapjaw</exampledoc>
/// <examplecapture>whenever a creature enters the battlefield under your control, if that creature has greater power or toughness than this creature, put a +1/+1 counter on this creature</examplecapture>
public class EvolveRule : Glyph
{
    public override Nib[] Nibs => ["whenever a creature enters the battlefield under your control, if that creature has greater power or toughness than this creature, put a +1/+1 counter on this creature"];
}
