namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An action whose object is picked at random rather than chosen: "deals 2 damage to any target chosen at random", "fights target creature an opponent controls chosen at random". The action is any predicate; "chosen at random" applies to the object it ends with.</summary>
/// <exampledoc>Goblin Test Pilot</exampledoc>
/// <examplecapture>deals 2 damage to any target chosen at random</examplecapture>
[Dependent]
public class ObjectChosenAtRandom : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Action), "chosen at random"];

    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Action { get; set; }
}
