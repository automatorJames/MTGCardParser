namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect done only if a condition holds, the condition trailing: "put that card into your hand if it's a land", "put a +1/+1 counter on it if it's a creature", "untap all creatures you control if {w} was spent to cast this spell".</summary>
/// <exampledoc>Dire Fleet Interloper</exampledoc>
/// <examplecapture>put that card into your hand if it's a land</examplecapture>
public class EffectIfCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Effect), "if", Prop(Condition)];

    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Effect { get; set; }
    public OneOf<CardIsKind, TransformedType, ManaWasSpent> Condition { get; set; }
}
