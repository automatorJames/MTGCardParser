namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect that adds to the spell's usual one, opening with "also": "also regenerate each other creature you control", "also create x 4/4 white angel creature tokens with flying", "also put a +1/+1 counter on each other creature you control". With no subject, the predicate is an instruction to the ability's controller. Usually the effect of an "if …," condition (spell mastery, kicker, gift); a leading "instead" is <see cref="InsteadConditional"/>.</summary>
/// <exampledoc>Dark Dabbling</exampledoc>
/// <examplecapture>also regenerate each other creature you control</examplecapture>
public class AdditionalEffect : Glyph
{
    public override Nib[] Nibs => ["also", Prop(Subject), Prop(Predicate)];

    [Optional]
    public Subject Subject { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Predicate { get; set; }
}
