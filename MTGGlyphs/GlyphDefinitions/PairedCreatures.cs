namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A group of creatures named earlier in the line, as a subject: the two creatures of a soulbond pair ("both creatures"), or the creatures just mentioned ("each of those creatures").</summary>
/// <exampledoc>Wingcrafter</exampledoc>
/// <examplecapture>both creatures</examplecapture>
[Dependent]
public class PairedCreatures : Glyph
{
    public override Nib[] Nibs => [Alt("both creatures", "each of those creatures")];
}
