namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What devour counts: "creature it devoured" - the permanents sacrificed to this creature's devour ability as it entered.</summary>
/// <exampledoc>Skullmulcher</exampledoc>
/// <examplecapture>creature it devoured</examplecapture>
[Dependent]
public class PermanentsDevoured : Glyph
{
    public override Nib[] Nibs => [Plural(Prop(Kind)), "it devoured"];

    public PermanentKind Kind { get; set; }
}
