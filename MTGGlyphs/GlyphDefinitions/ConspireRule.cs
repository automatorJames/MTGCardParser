namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Conspire's reminder text.</summary>
/// <exampledoc>Wort, Boggart Auntie</exampledoc>
/// <examplecapture>as you cast this spell, you may tap two untapped creatures you control that share a color with it</examplecapture>
public class ConspireRule : Glyph
{
    public override Nib[] Nibs => ["as you cast this spell, you may tap two untapped creatures you control that share a color with it"];
}
