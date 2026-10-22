namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"or planeswalker" in "that creature or planeswalker": the second of two card types a back-reference names.</summary>
/// <exampledoc>Scorching Dragonfire</exampledoc>
/// <examplecapture>or planeswalker</examplecapture>
[Dependent]
public class OrCardType : Glyph
{
    public override Nib[] Nibs => ["or", Prop(Kind)];

    public CardType Kind { get; set; }
}
