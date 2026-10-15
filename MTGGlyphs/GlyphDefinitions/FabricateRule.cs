namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Fabricate's reminder text.</summary>
/// <exampledoc>Angel of Invention</exampledoc>
/// <examplecapture>when this creature enters the battlefield, put a +1/+1 counter on it or create a 1/1 colorless servo artifact creature token</examplecapture>
public class FabricateRule : Glyph
{
    public override Nib[] Nibs => ["when this creature enters the battlefield, put a +1/+1 counter on it or create a 1/1 colorless servo artifact creature token"];
}
