namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Damage to you as a trigger condition: "you're dealt damage" (the contraction PlayerEvent's player + event can't split). Resolved inside TriggeredAbility's condition.</summary>
/// <exampledoc>Angelheart Vial</exampledoc>
/// <examplecapture>you're dealt damage</examplecapture>
[Dependent]
public class YouAreDealtDamage : Glyph
{
    public override Nib[] Nibs => ["you're dealt damage"];
}
