namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Putting some of the cards just looked at or revealed in one place and the rest in another: "put one of them into your hand and the rest on the bottom of your library in any order"; said of a player, "that player puts that card onto the battlefield and all other cards revealed this way into their graveyard".</summary>
/// <exampledoc>Impulse</exampledoc>
/// <examplecapture>put one of them into your hand and the rest on the bottom of your library in any order</examplecapture>
public class PutSplitCards : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("puts?"), Prop(Split)];

    public SplitLookedAtCards Split { get; set; }
}
