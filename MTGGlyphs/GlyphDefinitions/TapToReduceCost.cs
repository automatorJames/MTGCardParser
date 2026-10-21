namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Harmonize's cost reduction: tapping a creature to pay part of a spell's cost, "you may tap a creature you control to reduce that cost by {x}" (x defined after it).</summary>
/// <exampledoc>Roamer's Routine</exampledoc>
/// <examplecapture>you may tap a creature you control to reduce that cost by {x}</examplecapture>
public class TapToReduceCost : Glyph
{
    public override Nib[] Nibs => ["you may tap", Prop(Tapped), "to reduce that cost by", Prop(Reduction)];

    public IndefinitePermanent Tapped { get; set; }
    public ManaCost Reduction { get; set; }
}
