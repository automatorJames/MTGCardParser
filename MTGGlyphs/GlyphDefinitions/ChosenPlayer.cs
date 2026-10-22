namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the chosen player", "the chosen opponent": a player chosen as the permanent entered or earlier in the ability, named again in a later line.</summary>
/// <exampledoc>Pallimud</exampledoc>
/// <examplecapture>the chosen player</examplecapture>
[Dependent]
public class ChosenPlayer : Glyph
{
    public override Nib[] Nibs => ["the chosen", Prop(Player)];

    public PlayerIdentity Player { get; set; }
}
