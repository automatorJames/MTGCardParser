namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target player's", "target opponent's": a targeted player in the possessive.</summary>
/// <exampledoc>Clairvoyance</exampledoc>
/// <examplecapture>target player's</examplecapture>
[Dependent]
public class TargetPlayersPossessive : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "'s"];

    public SpecificTarget Player { get; set; }
}
