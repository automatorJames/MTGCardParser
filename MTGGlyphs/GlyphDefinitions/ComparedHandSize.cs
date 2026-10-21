namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A hand size compared with another player's: "more cards in hand than each opponent", "fewer cards in hand than you".</summary>
/// <exampledoc>Death of a Thousand Stings</exampledoc>
/// <examplecapture>more cards in hand than each opponent</examplecapture>
[Dependent]
public class ComparedHandSize : Glyph
{
    public override Nib[] Nibs => [Prop(Comparison), "cards in hand than", Prop(Other)];

    public MoreOrFewer Comparison { get; set; }
    public OneOf<WhichPlayer?, PlayerReference> Other { get; set; }
}
