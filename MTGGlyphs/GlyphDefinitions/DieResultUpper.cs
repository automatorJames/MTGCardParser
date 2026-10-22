namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The top of a die-table row's range: "—9".</summary>
/// <exampledoc>Goblin Morningstar</exampledoc>
/// <examplecapture>—9</examplecapture>
[Dependent]
public class DieResultUpper : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["—", Prop(High)];

    public int High { get; set; }
}
