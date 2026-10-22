namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player about to draw a card under some circumstance, as a replacement effect's condition: "you would draw a card except the first one you draw in each of your draw steps", "you would draw a card while your library has no cards in it", "you would draw a card during your draw step". The plain "you would draw a card" is WouldHappen.</summary>
/// <exampledoc>Laboratory Maniac</exampledoc>
/// <examplecapture>you would draw a card while your library has no cards in it</examplecapture>
[Dependent]
public class WouldDrawWhen : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "would draw a card", Prop(When)];

    public Subject Player { get; set; }
    public DrawCircumstance When { get; set; }
}
