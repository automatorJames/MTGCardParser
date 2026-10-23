namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An any-mana rider as a sentence of its own, after the sentence granting the permission to cast: "mana of any type can be spent to cast those spells", "mana of any type can be spent to cast spells this way".</summary>
/// <exampledoc>Azula, Cunning Usurper</exampledoc>
/// <examplecapture>mana of any type can be spent to cast those spells</examplecapture>
public class SpendAnyManaRule : Glyph
{
    public override Nib[] Nibs => [Prop(Rider)];

    public SpendAnyMana Rider { get; set; }
}
