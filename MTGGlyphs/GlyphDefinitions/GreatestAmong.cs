namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the greatest power among creatures you control", "the greatest mana value among permanents you control", "the greatest toughness among other creatures you control", "the greatest mana value among cards in your graveyard", "the greatest power among creature cards in your graveyard", "the greatest mana value among cards exiled this way": the highest value of a characteristic in a group of permanents or cards (the frame records "other", excluding this permanent).</summary>
/// <exampledoc>The Great Henge</exampledoc>
/// <examplecapture>the greatest power among creatures you control</examplecapture>
[Dependent]
public class GreatestAmong : Glyph
{
    public override Nib[] Nibs => ["the greatest", Prop(Characteristic), "among", Opt("other"), Prop(Group)];

    public Characteristic Characteristic { get; set; }
    public OneOf<Permanents, CardsInZone, AffectedThisWay> Group { get; set; }
}
