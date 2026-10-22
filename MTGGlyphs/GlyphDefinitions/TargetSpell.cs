namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target spell", "target artifact spell", "target red or green spell", "target creature spell with mana value 4 or less", "target spell that targets a creature", "target spell you don't control", "up to one target spell", "up to two target spells": a spell on the stack, optionally of a color or type, or bounded. A referent, for a later "that spell" or "its controller".</summary>
/// <exampledoc>Counterspell</exampledoc>
/// <examplecapture>target spell</examplecapture>
[Dependent]
[Referent]
public class TargetSpell : Glyph
{
    public override Nib[] Nibs => [Prop(UpTo), "target", Prop(Qualifiers), Plural("spell"), Prop(Bound)];

    [Optional]
    public UpToCount UpTo { get; set; }
    [Optional]
    public OneOf<ManyOf<TargetQualifier>, QualifierRun> Qualifiers { get; set; }
    [Optional]
    public OneOf<ManaValueLimit, WithQualifier, OneOf<SpellTargets, ControlledBy>> Bound { get; set; }
}
