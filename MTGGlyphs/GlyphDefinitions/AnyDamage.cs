namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Damage with no amount given, as replacement and prevention effects speak of it: "damage", "combat damage" - "would deal damage to you", "would deal combat damage to a player".</summary>
/// <exampledoc>Dictate of the Twin Gods</exampledoc>
/// <examplecapture>damage</examplecapture>
[Dependent]
public class AnyDamage : Glyph
{
    public override Nib[] Nibs => [Prop(Combat), "damage"];

    [RegexPattern("combat")]
    public bool Combat { get; set; }
}
