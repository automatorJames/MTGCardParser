namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"would be put into your graveyard", "would be put into a graveyard", "would be put into your graveyard from anywhere this turn": a card about to go to a graveyard.</summary>
/// <exampledoc>Goblin Dark-Dwellers</exampledoc>
/// <examplecapture>would be put into your graveyard</examplecapture>
[Dependent]
public class WouldGoToGraveyard : Glyph
{
    public override Nib[] Nibs => ["would be put into", Prop(Whose), "graveyard", Prop(FromAnywhere), Prop(Period)];

    public Whose Whose { get; set; }
    [RegexPattern("from anywhere")]
    public bool FromAnywhere { get; set; }
    [Optional]
    public ThisPeriod Period { get; set; }
}
