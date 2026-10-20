namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Prepared's reminder text, first sentence.</summary>
/// <exampledoc>Emergency Phytomedic // Seed Suture</exampledoc>
/// <examplecapture>while it's prepared, you may cast a copy of its spell</examplecapture>
public class PreparedRule : Glyph
{
    public override Nib[] Nibs => ["while it's prepared, you may cast a copy of its spell"];
}
