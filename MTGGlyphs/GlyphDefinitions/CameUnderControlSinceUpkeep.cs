namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Echo's condition: "this came under your control since the beginning of your last upkeep".</summary>
/// <exampledoc>Ghitu Slinger</exampledoc>
/// <examplecapture>this came under your control since the beginning of your last upkeep</examplecapture>
[Dependent]
public class CameUnderControlSinceUpkeep : Glyph
{
    public override Nib[] Nibs => [Prop(Permanent), "came under your control since the beginning of your last upkeep"];

    public PermanentPhrase Permanent { get; set; }
}
