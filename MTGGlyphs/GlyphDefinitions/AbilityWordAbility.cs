namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An ability under an ability word: "landfall — whenever a land you control enters, …", "threshold — as long as …". The ability word only labels it; the ability after the dash is any glyph that reads it.</summary>
/// <exampledoc>Geyserfield Stalker</exampledoc>
/// <examplecapture>landfall — whenever a land you control enters, this creature gets +2/+2 until end of turn</examplecapture>
public class AbilityWordAbility : Glyph
{
    public override Nib[] Nibs => [Prop(Word), "—", Prop(Ability)];

    public AbilityWord Word { get; set; }
    public DynamicGlyph Ability { get; set; }
}
