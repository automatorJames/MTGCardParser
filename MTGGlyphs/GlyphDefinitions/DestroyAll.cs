namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "destroy all creatures blocking or blocked by it". Tried ahead of <see cref="DestroyAllCardType"/>, which would
/// otherwise claim its first three words and leave the rest unmatched.
/// </summary>
[TokenizationOrder(0)]
public class DestroyAll : Glyph
{
    public override Nib[] Nibs => ["destroy all", Prop(Destroyed)];

    public CreaturesBlockingOrBlockedBy Destroyed { get; set; }
}
