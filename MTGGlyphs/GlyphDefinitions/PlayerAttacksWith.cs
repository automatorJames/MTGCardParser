namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player attacking with a number of creatures, as a trigger condition: "you attack with one or more creatures", "you attack with three or more creatures", "a player attacks with one or more creatures", "you attack with two or more creatures with flying", or naming what's attacked, "you attack a player with one or more equipped creatures". Resolved inside TriggeredAbility's condition.</summary>
/// <exampledoc>Military Intelligence</exampledoc>
/// <examplecapture>you attack with two or more creatures</examplecapture>
[Dependent]
public class PlayerAttacksWith : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Alt("attack", "attacks"), Prop(Defender), "with", Prop(Attackers)];

    public WhichPlayer Player { get; set; }
    [Optional]
    public AttackedDefender? Defender { get; set; }
    public OneOf<OneOrMorePermanents, NOrMorePermanents> Attackers { get; set; }
}
