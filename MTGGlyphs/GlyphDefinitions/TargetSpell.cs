namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target spell", "target artifact spell": a spell on the stack, optionally of a color or type. A referent, for a later "that spell" or "its controller".</summary>
/// <exampledoc>Counterspell</exampledoc>
/// <examplecapture>target spell</examplecapture>
[Dependent]
[Referent]
public class TargetSpell : Glyph
{
    public override Nib[] Nibs => ["target", Prop(Qualifiers), "spell"];

    [Optional]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
}
