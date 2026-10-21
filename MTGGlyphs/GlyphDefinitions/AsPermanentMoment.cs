namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"as this creature enters": the moment something happens, as a trailing clause.</summary>
/// <exampledoc>Sunrise Cavalier</exampledoc>
/// <examplecapture>as this creature enters</examplecapture>
[Dependent]
public class AsPermanentMoment : Glyph
{
    public override Nib[] Nibs => ["as", Prop(Moment)];

    public PermanentAsEvent Moment { get; set; }
}
