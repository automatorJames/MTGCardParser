namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A multiplier, counting things: "for each creature blocking it beyond the first", "for each age counter on it". Should replace ForEachCounter in OptionalPayCost.</summary>
/// <exampledoc>Chromium</exampledoc>
/// <examplecapture>for each creature blocking it beyond the first</examplecapture>
[Dependent]
public class ForEach : Glyph
{
    public override Nib[] Nibs => ["for each", Prop(Counted)];

    public OneOf<CountersOn, BlockerBeyondFirst> Counted { get; set; }
}
