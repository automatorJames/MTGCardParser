namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Dash's reminder text, first sentence.</summary>
/// <exampledoc>Mardu Scout</exampledoc>
/// <examplecapture>you may cast this spell for its dash cost</examplecapture>
public class DashRule : Glyph
{
    public override Nib[] Nibs => ["you may cast this spell for its dash cost"];
}
