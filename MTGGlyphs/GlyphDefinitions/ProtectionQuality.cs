namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What protection is from: a chosen color ("the chosen color", "the color of your choice"), a kind of permanent in the plural ("artifacts", "goblins"), or a quality ("red", "multicolored").</summary>
/// <exampledoc>Infested Roothold</exampledoc>
/// <examplecapture>artifacts</examplecapture>
[Dependent]
public class ProtectionQuality : GlyphOneOf
{
    public ChosenQuality? Chosen { get; set; }
    public PluralKind Kinds { get; set; }
    public Quality Quality { get; set; }
}
