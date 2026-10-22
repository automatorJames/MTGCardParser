namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A rider on the objects an effect just acted on: "creatures destroyed this way can't be regenerated", "a creature dealt damage this way can't be regenerated this turn", "creatures dealt damage this way can't block this turn".</summary>
/// <exampledoc>Catastrophe</exampledoc>
/// <examplecapture>creatures destroyed this way can't be regenerated</examplecapture>
public class ThisWayRider : Glyph
{
    public override Nib[] Nibs => [Prop(Affected), Prop(Predicate)];

    public OneOf<AnAffectedThisWay, AffectedThisWay> Affected { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Predicate { get; set; }
}
