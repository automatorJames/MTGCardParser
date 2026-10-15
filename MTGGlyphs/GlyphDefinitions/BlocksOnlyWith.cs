namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Shadow's reminder predicate: "can block or be blocked by only creatures with shadow".</summary>
/// <exampledoc>Augur il-Vec</exampledoc>
/// <examplecapture>can block or be blocked by only creatures with shadow</examplecapture>
[Dependent]
public class BlocksOnlyWith : Glyph, IPredicate
{
    public override Nib[] Nibs => ["can block or be blocked by only creatures", Prop(Condition)];

    public WithKeywords Condition { get; set; }
}
