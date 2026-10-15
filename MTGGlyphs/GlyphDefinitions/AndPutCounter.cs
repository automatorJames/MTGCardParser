namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"and put a +1/+1 counter on it": the megamorph bonus.</summary>
/// <exampledoc>Ainok Survivalist</exampledoc>
/// <examplecapture>and put a +1/+1 counter on it</examplecapture>
[Dependent]
public class AndPutCounter : Glyph
{
    public override Nib[] Nibs => ["and", Prop(Put)];

    public PlaceCounters Put { get; set; }
}
