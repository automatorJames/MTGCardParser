namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The alternative to a preceding condition or choice: "otherwise, put it into your graveyard", "otherwise, it gets -3/-3", "otherwise, you may put it on the bottom of your library". With no subject, the predicate is an instruction to the ability's controller.</summary>
/// <exampledoc>Call of the Wild</exampledoc>
/// <examplecapture>otherwise, put it into your graveyard</examplecapture>
public class Otherwise : Glyph
{
    public override Nib[] Nibs => ["otherwise,", Prop(Subject), Prop(Predicate)];

    [Optional]
    public Subject Subject { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Predicate { get; set; }
}
