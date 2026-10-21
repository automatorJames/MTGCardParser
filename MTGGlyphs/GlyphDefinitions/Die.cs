namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent dying, as a predicate in the base form a replacement's condition uses: "(would) die", "(would) die this turn".</summary>
/// <exampledoc>Gravebane Zombie</exampledoc>
/// <examplecapture>die</examplecapture>
[Dependent]
public class Die : Glyph, IPredicate
{
    public override Nib[] Nibs => ["die", Prop(Period)];

    [Optional]
    public ThisPeriod Period { get; set; }
}
