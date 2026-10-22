namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A kind that can be targeted, singular or plural: "creatures", "players", "artifact" - one of a list in "two target artifacts or enchantments".</summary>
/// <exampledoc>Wear Down</exampledoc>
/// <examplecapture>artifacts</examplecapture>
[Dependent]
public class PluralTargetKind : Glyph
{
    public override Nib[] Nibs => [Plural(Prop(Kind))];

    public TargetKind Kind { get; set; }
}
