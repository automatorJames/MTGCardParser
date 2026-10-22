namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static rule setting or changing a player's maximum hand size: "your maximum hand size is two", "each opponent's maximum hand size is reduced by one", "your maximum hand size is increased by two", "your maximum hand size is reduced by three for the rest of the game".</summary>
/// <exampledoc>Gnat Miser</exampledoc>
/// <examplecapture>each opponent's maximum hand size is reduced by one</examplecapture>
public class MaximumHandSize : Glyph
{
    public override Nib[] Nibs => [Prop(Whose), "maximum hand size is", Prop(Size), Prop(Duration)];

    public OneOf<Whose?, PlayerPossessive> Whose { get; set; }
    public OneOf<HandSizeChange, Quantity?> Size { get; set; }
    [Optional]
    public ForTheRestOfTheGame Duration { get; set; }
}
