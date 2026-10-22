namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that entered this turn", "that entered the battlefield this turn": a permanent narrowed to those that entered the battlefield this turn.</summary>
/// <exampledoc>Gallia, the Merrymaker</exampledoc>
/// <examplecapture>that entered this turn</examplecapture>
[Dependent]
public class EnteredThisTurn : Glyph
{
    public override Nib[] Nibs => ["that entered", Opt("the battlefield"), "this turn"];
}
