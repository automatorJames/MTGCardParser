namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"your starting life total", "half your starting life total": the life a player began the game with, as a bound.</summary>
/// <exampledoc>Path of Bravery</exampledoc>
/// <examplecapture>your starting life total</examplecapture>
[Dependent]
public class StartingLifeTotal : Glyph
{
    public override Nib[] Nibs => [Prop(Half), Prop(Whose), "starting life total"];

    [RegexPattern("half")]
    public bool Half { get; set; }
    public Whose Whose { get; set; }
}
