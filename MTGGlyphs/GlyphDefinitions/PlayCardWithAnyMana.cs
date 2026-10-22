namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Permission to cast or play a card with the any-mana rider: "cast that card for as long as it remains exiled, and mana of any type can be spent to cast that spell".</summary>
/// <exampledoc>Gonti, Lord of Luxury</exampledoc>
/// <examplecapture>cast that card for as long as it remains exiled, and mana of any type can be spent to cast that spell</examplecapture>
[Dependent]
public class PlayCardWithAnyMana : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Play), ", and", Prop(Mana)];

    public PlayCard Play { get; set; }
    public SpendAnyMana Mana { get; set; }
}
