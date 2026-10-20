namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Evolve's reminder text ("enters" in current wording, "enters the battlefield under your control" in older).</summary>
/// <exampledoc>Adaptive Snapjaw</exampledoc>
/// <examplecapture>whenever a creature you control enters, if that creature has greater power or toughness than this creature, put a +1/+1 counter on this creature</examplecapture>
public class EvolveRule : Glyph
{
    public override Nib[] Nibs => ["whenever a creature", Alt("you control enters", "enters the battlefield under your control"), ", if that creature has greater power or toughness than this creature, put a +1/+1 counter on this creature"];
}
