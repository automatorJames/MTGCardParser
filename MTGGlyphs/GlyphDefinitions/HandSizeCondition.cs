namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player's hand size as a condition (hellbent, Library of Alexandria): "you have no cards in hand", "that player has seven or more cards in hand", "you have more cards in hand than each opponent".</summary>
/// <exampledoc>Gathan Raiders</exampledoc>
/// <examplecapture>you have no cards in hand</examplecapture>
[Dependent]
public class HandSizeCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Verb), Prop(Size)];

    public OneOf<WhichPlayer?, PlayerReference> Player { get; set; }
    public HaveVerb Verb { get; set; }
    public OneOf<CardsInHandCount, ComparedHandSize> Size { get; set; }
}
