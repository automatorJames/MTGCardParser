namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Behold's reminder text: "to behold a dragon, choose a dragon you control or reveal a dragon card from your hand".</summary>
/// <exampledoc>Sarkhan, Dragon Ascendant</exampledoc>
/// <examplecapture>to behold a dragon, choose a dragon you control or reveal a dragon card from your hand</examplecapture>
public class BeholdReminder : Glyph
{
    public override Nib[] Nibs => ["to behold", Prop(Kind), ", choose", Prop(Chosen), "you control or reveal", Prop(Revealed), "card from your hand"];

    public ArticleKind Kind { get; set; }
    public ArticleKind Chosen { get; set; }
    public ArticleKind Revealed { get; set; }
}
