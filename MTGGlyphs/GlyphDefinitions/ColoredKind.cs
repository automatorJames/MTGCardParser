namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"black zombie", "white angel": a permanent kind with a color, singular or plural.</summary>
/// <exampledoc>Ghoulflesh</exampledoc>
/// <examplecapture>black zombie</examplecapture>
[Dependent]
public class ColoredKind : Glyph
{
    public override Nib[] Nibs => [Prop(Color), Plural(Prop(Kind))];

    public ManaColor Color { get; set; }
    public PermanentKind Kind { get; set; }
}
