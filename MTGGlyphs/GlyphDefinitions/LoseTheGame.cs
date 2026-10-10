namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player losing the game outright: "you lose the game".</summary>
/// <exampledoc>Lich</exampledoc>
/// <examplecapture>lose the game</examplecapture>
[Dependent]
public class LoseTheGame : Glyph, IPredicate
{
    public override Nib[] Nibs => [Alt("lose", "loses"), "the game"];
}
