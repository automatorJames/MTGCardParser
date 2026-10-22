namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Changing a spell's targets: "you may choose new targets for the copy", "you may choose a new target for it", "you may choose new targets for the copies" (storm's reminder), "you may choose new targets for target spell", "that player may choose new targets for the copy". A predicate, so the player and "may" come from SubjectDoes. Replaces the literal ChooseNewTargetsForCopy and StormCopies.</summary>
/// <exampledoc>Redirect</exampledoc>
/// <examplecapture>choose new targets for target spell</examplecapture>
public class ChooseNewTargets : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("chooses?"), Alt("new targets", "a new target"), "for", Prop(Spell)];

    public OneOf<RetargetedObject?, TargetSpell> Spell { get; set; }
}
