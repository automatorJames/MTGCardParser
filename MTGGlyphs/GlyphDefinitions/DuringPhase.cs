namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"during your upkeep", "during your turn": throughout one player's step, phase or turn.</summary>
/// <exampledoc>Colossus of Sardia</exampledoc>
/// <examplecapture>during your upkeep</examplecapture>
[Dependent]
public class DuringPhase : Glyph
{
    public override Nib[] Nibs => ["during", Prop(Step)];

    public PlayersPhase Step { get; set; }
}
