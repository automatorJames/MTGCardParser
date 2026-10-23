namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A copy given the copier's own name, perhaps with one more change: "except its name is {this}", "except its name is {this} and it's legendary in addition to its other types", "except his name is {this} and he's a legendary human mercenary villain creature".</summary>
/// <exampledoc>Sarkhan, Soul Aflame</exampledoc>
/// <examplecapture>except its name is {this} and it's legendary in addition to its other types</examplecapture>
[Dependent]
public class CopyRenamed : Glyph
{
    public override Nib[] Nibs => ["except", Prop(Whose), "name is", Nib.This, Prop(OtherChange)];

    public CopyPossessive Whose { get; set; }
    [Optional]
    public CopyRenamedAlso OtherChange { get; set; }
}
