namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"or activate abilities that aren't mana abilities": barring abilities alongside spells, as split second does.</summary>
/// <exampledoc>Extirpate</exampledoc>
/// <examplecapture>or activate abilities that aren't mana abilities</examplecapture>
[Dependent]
public class OrActivateAbilities : Glyph
{
    public override Nib[] Nibs => ["or activate", Prop(Abilities)];

    public BarredAbilities Abilities { get; set; }
}
