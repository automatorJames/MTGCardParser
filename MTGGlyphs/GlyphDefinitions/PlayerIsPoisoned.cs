namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player having one or more poison counters, as a condition: "an opponent is poisoned", "defending player is poisoned", "its controller is poisoned".</summary>
/// <exampledoc>Septic Rats</exampledoc>
/// <examplecapture>defending player is poisoned</examplecapture>
[Dependent]
public class PlayerIsPoisoned : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "is poisoned"];

    public OneOf<WhichPlayer?, PlayerReference> Player { get; set; }
}
