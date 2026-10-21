namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Tiered's reminder: "choose one additional cost". Held by ReminderText after the tiered keyword.</summary>
/// <exampledoc>Fire Magic</exampledoc>
/// <examplecapture>choose one additional cost</examplecapture>
[Dependent]
public class TieredRule : Glyph
{
    public override Nib[] Nibs => ["choose one additional cost"];
}
