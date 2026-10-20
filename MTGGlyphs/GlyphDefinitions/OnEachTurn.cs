namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"on each of your turns", "on each of their turns": something allowed or repeated every turn of a player.</summary>
/// <exampledoc>Exploration</exampledoc>
/// <examplecapture>on each of your turns</examplecapture>
[Dependent]
public class OnEachTurn : Glyph
{
    public override Nib[] Nibs => ["on each of", Prop(Whose), "turns"];

    public Whose Whose { get; set; }
}
