namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which abilities a player is barred from activating: "abilities that aren't mana abilities", "abilities that aren't mana abilities or loyalty abilities", "abilities of artifacts, creatures, or enchantments", "abilities of cards in graveyards", or just "abilities".</summary>
/// <exampledoc>Extirpate</exampledoc>
/// <examplecapture>abilities that aren't mana abilities</examplecapture>
[Dependent]
public class BarredAbilities : Glyph
{
    public override Nib[] Nibs => ["abilities", Prop(Which)];

    [Optional]
    public OneOf<NonManaAbilities, AbilitiesOfKinds> Which { get; set; }
}
