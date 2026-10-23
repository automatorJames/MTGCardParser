namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A targetable kind named in the plural: "creatures", "golems", "permanents", "players".</summary>
/// <exampledoc>Radiate</exampledoc>
/// <examplecapture>permanents</examplecapture>
[Dependent]
public class PluralTargetKind : Glyph
{
    public override Nib[] Nibs => [Plural(Prop(Kind))];

    public TargetKind Kind { get; set; }
}
