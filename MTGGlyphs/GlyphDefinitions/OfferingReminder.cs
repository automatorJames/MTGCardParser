namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The offering keyword with its reminder text, first sentence: "rat offering (you may cast this spell any time you could cast an instant by sacrificing a rat and paying the difference in mana costs between this and the sacrificed rat". The creature type appears three times; all three are captured.</summary>
/// <exampledoc>Patron of the Nezumi</exampledoc>
/// <examplecapture>rat offering (you may cast this spell any time you could cast an instant by sacrificing a rat and paying the difference in mana costs between this and the sacrificed rat</examplecapture>
public class OfferingReminder : Glyph
{
    public override Nib[] Nibs => [Prop(Kind), "offering (you may cast this spell any time you could cast an instant by sacrificing", Pattern("an?"), Prop(Sacrificed), "and paying the difference in mana costs between this and the sacrificed", Prop(SacrificedAgain)];

    public CreatureType Kind { get; set; }
    public CreatureType Sacrificed { get; set; }
    public CreatureType SacrificedAgain { get; set; }
}
