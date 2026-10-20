namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you discarded a nonland card", "you discarded a creature card": a condition on what was discarded by the effect just described (connive, rummaging).</summary>
/// <exampledoc>Raffine's Informant</exampledoc>
/// <examplecapture>you discarded a nonland card</examplecapture>
[Dependent]
public class YouDiscardedCard : Glyph
{
    public override Nib[] Nibs => ["you discarded", Prop(Card)];

    public CardOfKind Card { get; set; }
}
