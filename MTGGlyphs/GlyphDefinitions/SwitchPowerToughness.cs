namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Switching a creature's power and toughness for a time: "switch target creature's power and toughness until end of turn", "switch this creature's power and toughness until end of turn", "switch its power and toughness until end of turn".</summary>
/// <exampledoc>About Face</exampledoc>
/// <examplecapture>switch target creature's power and toughness until end of turn</examplecapture>
public class SwitchPowerToughness : Glyph, IPredicate
{
    public override Nib[] Nibs => ["switch", Prop(Creature), "power and toughness", Prop(Duration)];

    public OneOf<Its, ObjectPossessive, TargetPossessive> Creature { get; set; }
    public EffectDuration Duration { get; set; }
}
