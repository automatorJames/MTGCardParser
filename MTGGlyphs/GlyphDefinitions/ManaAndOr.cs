namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two mana symbols either or both of which may be chosen: "{r} and/or {g}", the colors a hybrid mana ability splits among.</summary>
/// <exampledoc>Fungal Reaches</exampledoc>
/// <examplecapture>{r} and/or {g}</examplecapture>
[Dependent]
public class ManaAndOr : Glyph
{
    public override Nib[] Nibs => [Prop(First), "and/or", Prop(Second)];

    public ManaCost First { get; set; }
    public ManaCost Second { get; set; }
}
