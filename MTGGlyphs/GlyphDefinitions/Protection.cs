namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The protection keyword with its quality: "protection from red", "protection from artifacts", "protection from goblins", "protection from the chosen color", "protection from the color of your choice", "protection from black and from red".</summary>
/// <exampledoc>White Ward</exampledoc>
/// <examplecapture>protection from white</examplecapture>
[Dependent]
public class Protection : Glyph
{
    public override Nib[] Nibs => ["protection from", Prop(From), Prop(AndFrom)];

    public ProtectionQuality From { get; set; }
    [Optional]
    public AndFromQuality AndFrom { get; set; }
}
