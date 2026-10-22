namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"becomes blocked" / "become blocked": an effect making attacking creatures blocked, even ones that can't be blocked.</summary>
/// <exampledoc>Fog Patch</exampledoc>
/// <examplecapture>become blocked</examplecapture>
[Dependent]
public class BecomeBlocked : Glyph, IPredicate
{
    public override Nib[] Nibs => [Alt("becomes", "become"), "blocked"];
}
