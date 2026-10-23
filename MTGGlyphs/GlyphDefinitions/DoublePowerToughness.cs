namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Doubling a creature's power, or its power and toughness, usually for a time: "double target creature's power and toughness until end of turn", "double its power until end of turn", "double the power and toughness of each creature you control until end of turn".</summary>
/// <exampledoc>Bulk Up</exampledoc>
/// <examplecapture>double target creature's power until end of turn</examplecapture>
public class DoublePowerToughness : Glyph, IPredicate
{
    public override Nib[] Nibs => ["double", Prop(Creature), Opt("the"), "power", Prop(AndToughness), Prop(Doubled), Prop(Duration)];

    [Optional]
    public OneOf<Its, ObjectPossessive, TargetPossessive> Creature { get; set; }
    [RegexPattern("and toughness")]
    public bool AndToughness { get; set; }
    [Optional]
    public DoubledObjects Doubled { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
