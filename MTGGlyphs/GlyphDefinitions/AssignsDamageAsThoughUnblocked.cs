namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Assigning combat damage as though unblocked: "assign its combat damage as though it weren't blocked", "assign their combat damage this turn as though they weren't blocked". A predicate, so "you may have this creature …" and plain subjects take it.</summary>
/// <exampledoc>Lone Wolf</exampledoc>
/// <examplecapture>assign its combat damage as though it weren't blocked</examplecapture>
[Dependent]
public class AssignsDamageAsThoughUnblocked : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("assigns?"), Prop(Owner), "combat damage", Prop(Duration), "as though", Prop(Bearer), "weren't blocked"];

    public OneOf<Its, Their> Owner { get; set; }
    [Optional]
    public ThisPeriod Duration { get; set; }
    public OneOf<It, They> Bearer { get; set; }
}
