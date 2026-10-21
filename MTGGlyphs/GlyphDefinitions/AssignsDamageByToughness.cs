namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The Doran rule: "assigns combat damage equal to its toughness rather than its power", "assign combat damage equal to their toughness rather than their power". A predicate, so any subject takes it.</summary>
/// <exampledoc>Doran, the Siege Tower</exampledoc>
/// <examplecapture>assigns combat damage equal to its toughness rather than its power</examplecapture>
[Dependent]
public class AssignsDamageByToughness : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("assigns?"), "combat damage equal to", Prop(Owner), "toughness rather than", Prop(OwnerAgain), "power"];

    public OneOf<Its, Their> Owner { get; set; }
    public OneOf<Its, Their> OwnerAgain { get; set; }
}
