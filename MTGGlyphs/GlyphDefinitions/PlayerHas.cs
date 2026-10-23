namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that player has", "each opponent has": a player reference before a perfect-tense verb.</summary>
/// <exampledoc>Damping Sphere</exampledoc>
/// <examplecapture>that player has</examplecapture>
[Dependent]
public class PlayerHas : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "has"];

    public PlayerReference Player { get; set; }
}
