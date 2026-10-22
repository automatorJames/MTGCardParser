namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the number of islands you control", "the number of creatures you control": a count of one kind of permanent by controller, without further qualifiers - small enough to sit inside a with-qualifier (the full <see cref="NumberOf"/> would cycle back through it).</summary>
/// <exampledoc>Engulf the Shore</exampledoc>
/// <examplecapture>the number of islands you control</examplecapture>
[Dependent]
public class ControlledKindCount : Glyph
{
    public override Nib[] Nibs => ["the number of", Prop(Kind), Prop(Controller)];

    public PluralKind Kind { get; set; }
    public ControlledBy Controller { get; set; }
}
