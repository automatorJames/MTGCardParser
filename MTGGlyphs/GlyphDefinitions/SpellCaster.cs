namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you cast", "your opponents cast": who casts the spells meant.</summary>
/// <exampledoc>Baral, Chief of Compliance</exampledoc>
/// <examplecapture>you cast</examplecapture>
[Dependent]
public class SpellCaster : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Pattern("casts?")];

    public WhichPlayer Player { get; set; }
}
