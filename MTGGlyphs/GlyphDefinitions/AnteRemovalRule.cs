namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The ante rule on cards that move cards between players: "remove this card from your deck before playing if you're not playing for ante".</summary>
/// <exampledoc>Amulet of Quoz</exampledoc>
/// <examplecapture>remove this card from your deck before playing if you're not playing for ante</examplecapture>
public class AnteRemovalRule : Glyph
{
    public override Nib[] Nibs => ["remove", Prop(Card), "from your deck before playing if you're not playing for ante"];

    public OneOf<This, ThisCreature> Card { get; set; }
}
