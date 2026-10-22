namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A plain "you control", "you don't control", "an opponent controls", "they control": who controls something, with no back-referring player - for back-references like "the creature you control", where <see cref="ControlledBy"/> would loop back through them.</summary>
/// <exampledoc>Malamet Battle Glyph</exampledoc>
/// <examplecapture>you control</examplecapture>
[Dependent]
public class PlainControlledBy : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Prop(Negated), Prop(Relation)];

    public OneOf<TheyPlayer, WhichPlayer?> Controller { get; set; }
    [RegexPattern("don't|doesn't")]
    public bool Negated { get; set; }
    public ControlRelation Relation { get; set; }
}
