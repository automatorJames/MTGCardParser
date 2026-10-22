namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"except the first one you draw in each of your draw steps": a draw replacement that spares each turn's normal draw.</summary>
/// <exampledoc>Alhammarret's Archive</exampledoc>
/// <examplecapture>except the first one you draw in each of your draw steps</examplecapture>
[Dependent]
public class ExceptFirstDraw : Glyph
{
    public override Nib[] Nibs => ["except the first one", Alt("you", "they"), "draw in each of", Alt("your", "their"), "draw steps"];
}
