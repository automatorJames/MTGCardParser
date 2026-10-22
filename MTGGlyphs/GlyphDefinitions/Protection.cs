namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Protection or hexproof from a quality: "protection from red", "protection from artifacts", "protection from goblins", "protection from the chosen color", "protection from the color of your choice", "protection from black and from red", "hexproof from monocolored", "hexproof from planeswalkers".</summary>
/// <exampledoc>White Ward</exampledoc>
/// <examplecapture>protection from white</examplecapture>
[Dependent]
public class Protection : Glyph
{
    public override Nib[] Nibs => [Prop(Kind), "from", Prop(From), Prop(AndFrom)];

    public FromKeyword Kind { get; set; }
    public ProtectionQuality From { get; set; }
    [Optional]
    public AndFromQuality AndFrom { get; set; }
}
