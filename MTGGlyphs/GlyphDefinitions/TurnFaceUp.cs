namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Morph's second reminder sentence: "turn it face up any time for its morph cost", or megamorph's, which adds a counter.</summary>
/// <exampledoc>Aquamorph Entity</exampledoc>
/// <examplecapture>turn it face up any time for its morph cost</examplecapture>
public class TurnFaceUp : Glyph
{
    public override Nib[] Nibs => ["turn it face up any time for its", Alt("morph", "megamorph"), "cost", Prop(Counter)];

    [Optional]
    public AndPutCounter Counter { get; set; }
}
