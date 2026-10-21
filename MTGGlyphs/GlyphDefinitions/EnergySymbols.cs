namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Energy written as symbols: "{e}{e}{e}".</summary>
/// <exampledoc>Aether Chaser</exampledoc>
/// <examplecapture>{e}{e}</examplecapture>
[Dependent]
public class EnergySymbols : Glyph
{
    public override Nib[] Nibs => [Pattern(@"(\{e\})+")];
}
