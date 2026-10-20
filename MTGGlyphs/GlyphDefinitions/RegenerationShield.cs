namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Regeneration's reminder text: "the next time this creature would be destroyed this turn, instead tap it, remove it from combat, and heal all damage on it" (current wording), or "…, it isn't" (older).</summary>
/// <exampledoc>Drudge Skeletons</exampledoc>
/// <examplecapture>the next time this creature would be destroyed this turn, instead tap it, remove it from combat, and heal all damage on it</examplecapture>
public class RegenerationShield : Glyph
{
    public override Nib[] Nibs => ["the next time", Prop(Shielded), "would be destroyed this turn,", Alt("instead tap it, remove it from combat, and heal all damage on it", "it isn't")];

    public OneOf<ThisCreature, ThatCard> Shielded { get; set; }
}
