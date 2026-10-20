namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"by this creature this turn", "by enchanted creature this turn": the source that dealt damage, and when.</summary>
/// <exampledoc>Frostwielder</exampledoc>
/// <examplecapture>by this creature this turn</examplecapture>
[Dependent]
public class DamageDealtBy : Glyph
{
    public override Nib[] Nibs => ["by", Prop(Source), Prop(Period)];

    public PermanentPhrase Source { get; set; }
    [Optional]
    public ThisPeriod Period { get; set; }
}
