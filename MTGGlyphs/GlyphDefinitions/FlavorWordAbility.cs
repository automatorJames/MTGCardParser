namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An ability under a one-off flavor word, which has no rules meaning: "keen senses — when this creature enters, draw a card", "fire breath — …". Like <see cref="AbilityWordAbility"/>, but the name is free text (one to five words); tried after it, so real ability words keep their lines.</summary>
/// <exampledoc>Owlbear</exampledoc>
/// <examplecapture>keen senses — when this creature enters, draw a card</examplecapture>
[TokenizationOrder(-1)]
public class FlavorWordAbility : Glyph
{
    public override Nib[] Nibs => [Pattern("[a-z][a-z'!-]*( [a-z][a-z'!-]*){0,4}"), "—", Prop(Ability)];

    public DynamicGlyph Ability { get; set; }
}
