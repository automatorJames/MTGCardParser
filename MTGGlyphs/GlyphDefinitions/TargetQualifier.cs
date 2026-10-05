namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One word narrowing what a target can be: "black", "artifact", or negated, "nonblack", "nonartifact".</summary>
[Dependent]
public class TargetQualifier : Glyph
{
    public override Joiner Joiner => Joiner.None;

    public override Nib[] Nibs => [Prop(IsNegated), Prop(Quality)];

    [RegexPattern("non")]
    public bool IsNegated { get; set; }

    public OneOf<CardType?, ManaColor?> Quality { get; set; }
}
