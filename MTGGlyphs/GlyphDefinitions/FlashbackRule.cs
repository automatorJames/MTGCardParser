namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The reminder text of a keyword that lets a card be cast from the graveyard for an alternative cost: "you may cast this card from your graveyard for its flashback cost", "… transformed for its disturb cost", "… for its escape cost", "… for its flashback cost and any additional costs". The keyword is captured.</summary>
/// <exampledoc>Ancient Grudge</exampledoc>
/// <examplecapture>you may cast this card from your graveyard for its flashback cost</examplecapture>
public class FlashbackRule : Glyph
{
    public override Nib[] Nibs => ["you may cast this card from your graveyard", Prop(Transformed), "for its", Prop(Keyword), "cost", Prop(AdditionalCosts)];

    [RegexPattern("transformed")]
    public bool Transformed { get; set; }
    public Keyword Keyword { get; set; }
    [RegexPattern("and any additional costs")]
    public bool AdditionalCosts { get; set; }
}
