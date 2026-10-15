namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Looking at cards: "look at the top four cards of your library, then put them back in any order", "look at target player's hand". Tried after glyphs like ScryOneLook that spell out the same words.</summary>
/// <exampledoc>Aven Fateshaper</exampledoc>
/// <examplecapture>look at the top four cards of your library, then put them back in any order</examplecapture>
[TokenizationOrder(-1)]
public class LookAt : Glyph
{
    public override Nib[] Nibs => ["look at", Prop(What), Prop(PutBack)];

    public LookedAtThing What { get; set; }
    [Optional]
    public PutThemBack PutBack { get; set; }
}
