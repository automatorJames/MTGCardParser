namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// A static ability that holds while something is, or isn't, something: "as long as enchanted artifact isn't a
/// creature, …", "as long as {this} is untapped, …". Now a narrower case of WhileConditionStatic (whose PermanentIs condition covers it); tried first so its lines keep their parse until the two are merged.
/// </summary>
/// <exampledoc>Animate Artifact</exampledoc>
/// <examplecapture>as long as enchanted artifact isn't a creature, it's an artifact creature with power and toughness each equal to its mana value</examplecapture>
[TokenizationOrder(0)]
public class AsLongAs : Glyph
{
    public override Nib[] Nibs => ["as long as", Prop(Subject), Prop(Assertion), Opt(Pattern("an?")), Prop(Aspect), ",", Prop(Effect)];

    public OneOf<EnchantedPermanent, This> Subject { get; set; }
    public Assertion Assertion { get; set; }
    public CardAspect Aspect { get; set; }
    public DynamicGlyph Effect { get; set; }
}
