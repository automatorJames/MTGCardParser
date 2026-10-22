namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An activated ability with two alternative sets of costs, either of which pays: "{3}, {t} or {u}, {t}: return target creature to its owner's hand unless its controller pays {1}".</summary>
/// <exampledoc>Crystal Shard</exampledoc>
/// <examplecapture>{3}, {t} or {u}, {t}: return target creature to its owner's hand unless its controller pays {1}</examplecapture>
public class AlternativeCostsAbility : Glyph
{
    public override Nib[] Nibs => [Prop(Costs), "or", Prop(OtherCosts), ":", Prop(Effect)];

    public CompoundOf<Cost> Costs { get; set; }
    public CompoundOf<Cost> OtherCosts { get; set; }
    public DynamicGlyph Effect { get; set; }
}
