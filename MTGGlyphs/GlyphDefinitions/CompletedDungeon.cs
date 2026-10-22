namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player having completed a dungeon at some point in the game, as a condition: "you've completed a dungeon" (the venture/dungeon mechanic).</summary>
/// <exampledoc>Gloom Stalker</exampledoc>
/// <examplecapture>you've completed a dungeon</examplecapture>
[Dependent]
public class CompletedDungeon : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "completed a dungeon"];

    public OneOf<PerfectSubject?, WhichPlayer?> Player { get; set; }
}
