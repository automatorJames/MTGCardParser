namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Paying a cost once per object, as the second of two actions: "… and pay {2} for each creature chosen this way". Only a conjoined predicate, so a lone "pay {1}" stays an OptionalPayCost.</summary>
/// <exampledoc>Dream Tides</exampledoc>
/// <examplecapture>pay {2} for each creature chosen this way</examplecapture>
[Dependent]
public class PayPerEach : Glyph, IConjoinedPredicate
{
    public override Nib[] Nibs => [Pattern("pays?"), Prop(Cost), Prop(PerEach)];

    public Cost Cost { get; set; }
    public ForEach PerEach { get; set; }
}
