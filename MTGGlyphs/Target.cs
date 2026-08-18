namespace MTGGlyphs;

[Dependent]
public class Target : Glyph
{
    public override Nib[] Nibs => [Prop(IsAny), "target", Prop(TargetableEntity)];

    [RegexPattern("any")]
    public bool IsAny { get; set; }

    [Optional]
    public TargetableEntity TargetableEntity { get; set; }
}
