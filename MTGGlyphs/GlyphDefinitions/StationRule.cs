namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Station's reminder text: tapping creatures to charge a spacecraft or planet.</summary>
/// <exampledoc>Sledge-Class Seedship</exampledoc>
/// <examplecapture>tap another creature you control: put charge counters equal to its power on this spacecraft</examplecapture>
public class StationRule : Glyph
{
    public override Nib[] Nibs => ["tap another creature you control: put charge counters equal to its power on", Prop(Station)];

    public ThisCreature Station { get; set; }
}
