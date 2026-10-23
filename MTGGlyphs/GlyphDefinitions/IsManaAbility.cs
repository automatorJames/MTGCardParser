namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether a just-activated ability is a mana ability, as a condition: "if it isn't a mana ability" (copy and punish effects that skip mana abilities).</summary>
/// <exampledoc>Rings of Brighthearth</exampledoc>
/// <examplecapture>it isn't a mana ability</examplecapture>
[Dependent]
public class IsManaAbility : Glyph
{
    public override Nib[] Nibs => [Prop(Ability), Prop(Assertion), "a mana ability"];

    public It Ability { get; set; }
    public Assertion Assertion { get; set; }
}
