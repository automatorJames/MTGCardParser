namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a granted keyword's cost is: "the flashback cost is equal to its mana cost", "the flashback cost is equal to that card's mana cost" (Snapcaster Mage and its kin).</summary>
/// <exampledoc>Snapcaster Mage</exampledoc>
/// <examplecapture>the flashback cost is equal to its mana cost</examplecapture>
public class GrantedKeywordCost : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Keyword), "cost is equal to", Prop(Card), "mana cost"];

    public Keyword Keyword { get; set; }
    public OneOf<Its, ObjectPossessive> Card { get; set; }
}
