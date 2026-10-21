namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A count of distinct kinds rather than of objects: "basic land types among lands you control" (domain), "colors of mana spent to cast this spell" (converge).</summary>
/// <exampledoc>Exotic Disease</exampledoc>
/// <examplecapture>basic land types among lands you control</examplecapture>
[Dependent]
public class DistinctKinds : Glyph
{
    public override Nib[] Nibs => [Prop(Aspect), Alt("among", "of"), Prop(Among)];

    public CountedAspect Aspect { get; set; }
    public OneOf<Permanents, ManaSpentToCast> Among { get; set; }
}
