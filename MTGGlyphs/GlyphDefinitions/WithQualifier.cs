namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A "with …" qualifier on what's targeted: keywords ("with flying") or a bounded characteristic ("with power 2 or less"). A glyph wrapping a one-of, so it can be optional.</summary>
/// <exampledoc>Dwarven Warriors</exampledoc>
/// <examplecapture>with power 2 or less</examplecapture>
[Dependent]
public class WithQualifier : Glyph
{
    public override Nib[] Nibs => [Prop(Condition)];

    public OneOf<WithKeywords, WithCharacteristicBound> Condition { get; set; }
}
