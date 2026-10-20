namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that token", "the token", "those tokens": tokens created earlier in the line or ability.</summary>
/// <exampledoc>Geist of Saint Traft</exampledoc>
/// <examplecapture>that token</examplecapture>
[Dependent]
public class ThatToken : Glyph
{
    public override Nib[] Nibs => [Alt("that", "those", "the"), Plural("token")];
}
