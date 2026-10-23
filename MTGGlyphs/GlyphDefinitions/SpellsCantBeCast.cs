namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static ban on casting a group of spells: "spells with the chosen name can't be cast", "creature spells can't be cast", "noncreature spells with mana value 4 or greater can't be cast". The passive sibling of <see cref="CantCast"/>.</summary>
/// <exampledoc>Meddling Mage</exampledoc>
/// <examplecapture>spells with the chosen name can't be cast</examplecapture>
public class SpellsCantBeCast : Glyph
{
    public override Nib[] Nibs => [Prop(Spells), "can't be cast", Prop(ThisTurn)];

    public SpellsCast Spells { get; set; }
    [RegexPattern("this turn")]
    public bool ThisTurn { get; set; }
}
