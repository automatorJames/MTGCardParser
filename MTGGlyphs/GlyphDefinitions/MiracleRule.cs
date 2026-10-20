namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Miracle's reminder: "you may cast this card for its miracle cost when you draw it if it's the first card you drew this turn".</summary>
/// <exampledoc>Banishing Stroke</exampledoc>
/// <examplecapture>you may cast this card for its miracle cost when you draw it if it's the first card you drew this turn</examplecapture>
public class MiracleRule : Glyph
{
    public override Nib[] Nibs => ["you may cast this card for its miracle cost when you draw it if it's the first card you drew this turn"];
}
