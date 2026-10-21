namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A particular attacker a "can't block" restriction protects: "it", "this creature this turn", "creatures you control" - the object after "can't block", with the restriction's own duration after it.</summary>
/// <exampledoc>Screeching Griffin</exampledoc>
/// <examplecapture>this creature this turn</examplecapture>
[Dependent]
public class BlockedObject : Glyph
{
    public override Nib[] Nibs => [Prop(Object), Prop(Duration)];

    public OneOf<It, Permanents, ThisCreature> Object { get; set; }
    [Optional]
    public ThisPeriod Duration { get; set; }
}
