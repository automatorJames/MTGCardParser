namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Adding one creature's power or toughness to another's for a while: "add its power to this creature's until end of turn" (enlist).</summary>
/// <exampledoc>Hexbane Tortoise</exampledoc>
/// <examplecapture>add its power to this creature's until end of turn</examplecapture>
[Dependent]
public class AddCharacteristicTo : Glyph, IPredicate
{
    public override Nib[] Nibs => ["add", Prop(From), Prop(Characteristic), "to", Prop(To), Prop(Duration)];

    public OneOf<Its, ObjectPossessive> From { get; set; }
    public Characteristic Characteristic { get; set; }
    public ObjectPossessive To { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
