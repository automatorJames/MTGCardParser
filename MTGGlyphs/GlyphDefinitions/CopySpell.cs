namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Copying a spell or ability, often with new targets allowed: "copy this spell", "copy it and you may choose a new target for the copy", "(they may) copy this spell and may choose a new target for that copy", "copy that spell", "copy that ability", "copy target instant or sorcery spell you control", "copy target triggered ability", "copy it three times", "copy this spell except for its epic ability". Also the second of two actions.</summary>
/// <exampledoc>Rob the Archives</exampledoc>
/// <examplecapture>copy this spell</examplecapture>
public class CopySpell : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["copy", Prop(Spell), Prop(Times), Prop(Except), Prop(NewTargets)];

    public OneOf<CopiedSpell?, TargetSpell, TargetAbility> Spell { get; set; }
    [Optional]
    public CopyRepetition? Times { get; set; }
    [Optional]
    public ExceptForItsAbility Except { get; set; }
    [RegexPattern("and (you )?may choose (a new target|new targets) for (the|that) copy")]
    public bool NewTargets { get; set; }
}
