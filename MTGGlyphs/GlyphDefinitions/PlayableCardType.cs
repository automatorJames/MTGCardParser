namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card type named in the plural, as something to play: "lands".</summary>
/// <exampledoc>Crucible of Worlds</exampledoc>
/// <examplecapture>lands</examplecapture>
[Dependent]
public class PlayableCardType : Glyph
{
    public override Nib[] Nibs => [Plural(Prop(Kind))];

    public CardType Kind { get; set; }
}
