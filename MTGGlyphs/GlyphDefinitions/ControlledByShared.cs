namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"controlled by the same player", "controlled by different players": a targeting restriction tying several targets' controllers together, rather than naming the controller.</summary>
/// <exampledoc>Cannibalize</exampledoc>
/// <examplecapture>controlled by the same player</examplecapture>
[Dependent]
public class ControlledByShared : Glyph
{
    public override Nib[] Nibs => ["controlled by", Prop(Controllers)];

    public SharedController Controllers { get; set; }
}
