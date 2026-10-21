namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A multiplier, counting things: "for each creature blocking it beyond the first", "for each age counter on it", "for each artifact you control", "for each creature card in your graveyard", "for each color of mana spent to cast it", "for each creature tapped this way", "for each basic land type among lands you control", "for each target beyond the first", "for each time it was kicked", "for each creature that died this turn". Should replace ForEachCounter in OptionalPayCost.</summary>
/// <exampledoc>Chromium</exampledoc>
/// <examplecapture>for each creature blocking it beyond the first</examplecapture>
[Dependent]
public class ForEach : Glyph
{
    public override Nib[] Nibs => ["for each", Prop(Counted)];

    public CountedThing Counted { get; set; }
}
