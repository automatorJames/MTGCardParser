namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Mana of a color or type some permanent can make: "any color that a land you control could produce", "any type that a land an opponent controls could produce", "any type that land produced", "any type that a gate you control could produce", "any type that permanent produced".</summary>
/// <exampledoc>Harvester Druid</exampledoc>
/// <examplecapture>any color that a land you control could produce</examplecapture>
[Dependent]
public class ManaProducibleBy : Glyph
{
    public override Nib[] Nibs => ["any", Prop(Aspect), Prop(Producer), Prop(Tense)];

    public ManaAspect Aspect { get; set; }
    public OneOf<ThatIndefinitePermanent, ThatCard, ThatPermanentWord> Producer { get; set; }
    public ProductionTense Tense { get; set; }
}
