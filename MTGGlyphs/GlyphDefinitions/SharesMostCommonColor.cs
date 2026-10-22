namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A condition that an object shares a color with the most common color among all permanents, or one tied for it: "it shares a color with the most common color among all permanents or a color tied for most common".</summary>
/// <exampledoc>Heroic Defiance</exampledoc>
/// <examplecapture>it shares a color with the most common color among all permanents or a color tied for most common</examplecapture>
[Dependent]
public class SharesMostCommonColor : Glyph
{
    public override Nib[] Nibs => [Prop(Object), "shares a color with the most common color among all permanents or a color tied for most common"];

    public It Object { get; set; }
}
