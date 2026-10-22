namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Who chooses which object an effect applies to: "of their choice", "of an opponent's choice", "of defending player's choice", "of target opponent's choice".</summary>
/// <exampledoc>Clip Wings</exampledoc>
/// <examplecapture>of their choice</examplecapture>
[Dependent]
public class OfPlayersChoice : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Chooser), "choice"];

    public OneOf<Whose?, PlayerPossessive> Chooser { get; set; }
}
