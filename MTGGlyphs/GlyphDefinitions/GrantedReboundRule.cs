namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Rebound's reminder where rebound is granted to other spells: "exile the spell as it resolves if you cast it from your hand", "exile it as it resolves".</summary>
/// <exampledoc>Cast Through Time</exampledoc>
/// <examplecapture>exile the spell as it resolves if you cast it from your hand</examplecapture>
public class GrantedReboundRule : Glyph
{
    public override Nib[] Nibs => ["exile", Alt("the spell", "it"), "as it resolves", Prop(FromHand)];

    [RegexPattern("if you cast it from your hand")]
    public bool FromHand { get; set; }
}
