namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card previously exiled coming back: "that card returns to the battlefield (under its owner's control)" - champion's and flicker reminders.</summary>
/// <exampledoc>Changeling Berserker</exampledoc>
/// <examplecapture>that card returns to the battlefield</examplecapture>
public class CardReturnsToBattlefield : Glyph
{
    public override Nib[] Nibs => [Prop(Card), "returns to the battlefield", Prop(Control)];

    public ThatCardPhrase Card { get; set; }
    [Optional]
    public UnderControlOf Control { get; set; }
}
