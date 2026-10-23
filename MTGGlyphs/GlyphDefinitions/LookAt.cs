namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Looking at cards: "look at the top four cards of your library", "look at target player's hand", or in the third person, "(that player) looks at the top two cards of their library". Rearranging what was looked at is LookAndRearrange. Tried after glyphs like ScryOneLook that spell out the same words. Also a predicate, so "you may look at …" reads as SubjectDoes + MayDo, and the second of two actions.</summary>
/// <exampledoc>Natural Selection</exampledoc>
/// <examplecapture>look at the top three cards of target player's library</examplecapture>
[TokenizationOrder(-1)]
public class LookAt : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("looks? at"), Prop(What)];

    public LookedAtThing What { get; set; }
}
