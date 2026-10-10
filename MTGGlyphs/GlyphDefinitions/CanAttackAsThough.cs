namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"can attack as though it didn't have defender", "can attack as though it had haste".</summary>
/// <exampledoc>Animate Wall</exampledoc>
/// <examplecapture>can attack as though it didn't have defender</examplecapture>
[Dependent]
public class CanAttackAsThough : Glyph, IPredicate
{
    public override Nib[] Nibs => ["can attack as though it", Prop(Possession), Prop(Keyword)];

    public Possession Possession { get; set; }
    public Keyword Keyword { get; set; }
}
