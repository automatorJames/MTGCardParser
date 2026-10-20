namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Predefined token reminder text: "it's an artifact with "{t}, sacrifice this token: add one mana of any color"" (Treasure, Food, Clue, Blood, Map…), or "they're artifacts with …" after several: the token's type and the activated ability it has.</summary>
/// <exampledoc>Brazen Freebooter</exampledoc>
/// <examplecapture>it's an artifact with "{t}, sacrifice this token: add one mana of any color</examplecapture>
public class TokenHasAbility : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Alt("it's an artifact with \"", "they're artifacts with \""), Prop(Ability)];

    public ActivatedAbility Ability { get; set; }
}
