namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"less than or equal to the number of cards in your graveyard", "greater than or equal to your life total": a bound set by an amount from the game state.</summary>
/// <exampledoc>Ghastly Demise</exampledoc>
/// <examplecapture>less than or equal to the number of cards in your graveyard</examplecapture>
[Dependent]
public class BoundByAmount : Glyph
{
    public override Nib[] Nibs => [Prop(Comparison), "than or equal to", Prop(Amount)];

    public Comparison Comparison { get; set; }
    public Amount Amount { get; set; }
}
