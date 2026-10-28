namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object together with a group related to it, as one subject: "target artifact and all other artifacts with the same name as that artifact", "target creature and each other creature with the same name as that creature" (the Echoing cycle, Maelstrom Pulse), "enchanted creature and other creatures that share a creature type with it" (the Crown cycle).</summary>
/// <exampledoc>Echoing Ruin</exampledoc>
/// <examplecapture>target artifact and all other artifacts with the same name as that artifact</examplecapture>
[Dependent]
public class ObjectAndOthers : Glyph
{
    public override Nib[] Nibs => [Prop(Object), "and", Prop(Others)];

    public OneOf<SpecificTarget, EnchantedPermanent> Object { get; set; }
    public OneOf<PermanentGroup, EachPermanents, OtherPermanents> Others { get; set; }
}
