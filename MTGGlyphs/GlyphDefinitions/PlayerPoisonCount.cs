namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player's poison counters reaching a threshold, as a condition: "an opponent has three or more poison counters", "its controller has three or more poison counters" (corrupted).</summary>
/// <exampledoc>Apostle of Invasion</exampledoc>
/// <examplecapture>an opponent has three or more poison counters</examplecapture>
[Dependent]
public class PlayerPoisonCount : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Pattern("(has|have)"), Prop(Count), "poison counters"];

    public OneOf<WhichPlayer?, PlayerReference> Player { get; set; }
    public AtLeastCount Count { get; set; }
}
