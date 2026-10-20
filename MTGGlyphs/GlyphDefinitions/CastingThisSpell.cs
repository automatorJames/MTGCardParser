namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you cast this spell": the moment of casting, for casualty and other "as you cast" costs.</summary>
/// <exampledoc>Grisly Sigil</exampledoc>
/// <examplecapture>you cast this spell</examplecapture>
[Dependent]
public class CastingThisSpell : Glyph
{
    public override Nib[] Nibs => ["you cast this spell"];
}
