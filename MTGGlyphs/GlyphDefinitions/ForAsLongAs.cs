namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A duration tied to a condition: "for as long as {this} remains tapped", "for as long as you control {this} and {this} remains tapped", "for as long as it has a paralyzation counter on it".</summary>
/// <exampledoc>Ice Floe</exampledoc>
/// <examplecapture>for as long as {this} remains tapped</examplecapture>
[Dependent]
public class ForAsLongAs : Glyph
{
    public override Nib[] Nibs => ["for as long as", Prop(Conditions)];

    public OneOf<ManyOf<LastingCondition>, LastingCondition> Conditions { get; set; }
}
