namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"gift a card", "gift a tapped fish", "gift a food": the gift keyword with what the opponent gets.</summary>
/// <exampledoc>Dewdrop Cure</exampledoc>
/// <examplecapture>gift a card</examplecapture>
[Dependent]
public class GiftKeyword : Glyph
{
    public override Nib[] Nibs => ["gift", Prop(Gift)];

    public GiftKind Gift { get; set; }
}
