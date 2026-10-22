namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What protection is from: a chosen color ("the chosen color", "the color of your choice"), a kind of permanent in the plural ("artifacts", "goblins"), a quality ("red", "multicolored"), or a qualified group of permanents ("non-spirit creatures", "legendary creatures", "creatures your opponents control", "creatures of the chosen type").</summary>
/// <exampledoc>Infested Roothold</exampledoc>
/// <examplecapture>artifacts</examplecapture>
[Dependent]
public class ProtectionQuality : GlyphOneOf
{
    public ChosenQuality? Chosen { get; set; }
    public PluralKind Kinds { get; set; }
    public Quality Quality { get; set; }
    public Permanents Group { get; set; }
}
