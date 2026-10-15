namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Improvise's reminder text.</summary>
/// <exampledoc>Reverse Engineer</exampledoc>
/// <examplecapture>each artifact you tap after you're done activating mana abilities pays for {1}</examplecapture>
public class ImproviseRule : Glyph
{
    public override Nib[] Nibs => ["each artifact you tap after you're done activating mana abilities pays for {1}"];
}
