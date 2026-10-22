namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object's mana cost paid as a cost: "its mana cost", "that creature's mana cost", "its mana cost reduced by {2}".</summary>
/// <exampledoc>Pendrell Flux</exampledoc>
/// <examplecapture>its mana cost</examplecapture>
[Dependent]
public class ObjectManaCost : Glyph
{
    public override Nib[] Nibs => [Prop(Owner), "mana cost", Prop(Reduction)];

    public OneOf<Its, ObjectPossessive> Owner { get; set; }
    [Optional]
    public ManaCostReduction Reduction { get; set; }
}
