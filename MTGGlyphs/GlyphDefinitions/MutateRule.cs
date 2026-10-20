namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Mutate's reminder text, first sentence.</summary>
/// <exampledoc>Dreamtail Heron</exampledoc>
/// <examplecapture>if you cast this spell for its mutate cost, put it over or under target non-human creature you own</examplecapture>
public class MutateRule : Glyph
{
    public override Nib[] Nibs => ["if you cast this spell for its mutate cost, put it over or under target non-human creature you own"];
}
