namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The life players lost this turn, counted together: "the total life lost by your opponents this turn".</summary>
/// <exampledoc>Cryptborn Horror</exampledoc>
/// <examplecapture>the total life lost by your opponents this turn</examplecapture>
[Dependent]
public class TotalLifeLostBy : Glyph
{
    public override Nib[] Nibs => ["the total life lost by", Prop(Players), Prop(Turn)];

    public OneOf<WhichPlayer?, PlayerReference> Players { get; set; }
    public TurnSpan Turn { get; set; }
}
