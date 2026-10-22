namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Predefined token reminder text: "it's an artifact with "{t}, sacrifice this token: add one mana of any color"" (Treasure, Food, Clue, Blood, Map…), "they're artifacts with …" after several, naming the token, "a food token is an artifact with …", or a colored one, "it's a red and green artifact with "{t}: add {r} or {g}"" (Heartwood): the token's type and the activated ability it has.</summary>
/// <exampledoc>Brazen Freebooter</exampledoc>
/// <examplecapture>it's an artifact with "{t}, sacrifice this token: add one mana of any color</examplecapture>
public class TokenHasAbility : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Token), Prop(Ability)];

    public OneOf<TokenPronounIsArtifact?, NamedTokenIsArtifact, ColoredTokenIsArtifact> Token { get; set; }
    public ActivatedAbility Ability { get; set; }
}
