namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"and with flying", "and with first strike": a keyword a permanent enters with alongside its counters (the kicker volvers).</summary>
/// <exampledoc>Faerie Squadron</exampledoc>
/// <examplecapture>and with flying</examplecapture>
[Dependent]
public class AndEntersWithKeywords : Glyph
{
    public override Nib[] Nibs => ["and", Prop(With)];

    public WithKeywords With { get; set; }
}
