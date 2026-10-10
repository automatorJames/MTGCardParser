namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "lands", "nonblack creatures", "swamps", "tapped blue creatures they control": permanents of one kind (card type, land type or creature type), optionally narrowed
/// and optionally by who controls them. The kind is a referent, so a later "they" or "those creatures" refers to
/// them (see <see cref="ThatCard"/>).
/// </summary>
/// <exampledoc>Magnetic Mountain</exampledoc>
/// <examplecapture>tapped blue creatures they control</examplecapture>
[Dependent]
public class Permanents : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Plural(Prop(Kind)), Prop(Controller)];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    [Referent]
    [Plural]
    public PermanentKind Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
}
