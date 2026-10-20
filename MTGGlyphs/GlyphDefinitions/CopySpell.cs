namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Copying a spell, often with new targets allowed: "copy this spell", "copy it and you may choose a new target for the copy", "copy that spell".</summary>
/// <exampledoc>Rob the Archives</exampledoc>
/// <examplecapture>copy this spell</examplecapture>
public class CopySpell : Glyph, IPredicate
{
    public override Nib[] Nibs => ["copy", Prop(Spell), Prop(NewTargets)];

    public CopiedSpell Spell { get; set; }
    [RegexPattern("and you may choose (a new target|new targets) for the copy")]
    public bool NewTargets { get; set; }
}
