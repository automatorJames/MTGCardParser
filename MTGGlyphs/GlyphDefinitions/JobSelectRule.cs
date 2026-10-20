namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Job select's reminder text.</summary>
/// <exampledoc>Dark Knight's Greatsword</exampledoc>
/// <examplecapture>when this equipment enters, create a 1/1 colorless hero creature token, then attach this to it</examplecapture>
public class JobSelectRule : Glyph
{
    public override Nib[] Nibs => ["when this equipment enters, create a 1/1 colorless hero creature token, then attach this to it"];
}
