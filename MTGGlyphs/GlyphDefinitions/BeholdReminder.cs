namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Behold's reminder text: "to behold a dragon, choose a dragon you control or reveal a dragon card from your hand", or the short form after "you may behold a dragon": "you may choose a dragon you control or reveal a dragon card from your hand".</summary>
/// <exampledoc>Sarkhan, Dragon Ascendant</exampledoc>
/// <examplecapture>to behold a dragon, choose a dragon you control or reveal a dragon card from your hand</examplecapture>
public class BeholdReminder : Glyph
{
    public override Nib[] Nibs => [Prop(Intro), Opt("you may"), "choose", Prop(Chosen), "you control or reveal", Prop(Revealed), "card from your hand"];

    [Optional]
    public BeholdIntro Intro { get; set; }
    public ArticleKind Chosen { get; set; }
    public ArticleKind Revealed { get; set; }
}
