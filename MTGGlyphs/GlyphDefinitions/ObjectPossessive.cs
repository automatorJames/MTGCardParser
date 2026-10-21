namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object's possessive: "that creature's", "{this}'s", "this creature's", "the sacrificed artifact's", "the discarded card's", "that card's", "that spell's", "enchanted creature's". The spell reference goes before ThisCreature, whose bare "this" would stop short of "this spell's"; "that card"/"the exiled card" go before handled objects.</summary>
/// <exampledoc>Diamond Valley</exampledoc>
/// <examplecapture>the sacrificed creature's</examplecapture>
[Dependent]
public class ObjectPossessive : Glyph
{
    public override Nib[] Nibs => [Prop(Object), "'s"];

    public OneOf<This, ThatCard, OneOf<ThatCardPhrase, SacrificedPermanent, OneOf<SpellReference?, ThisCreature, EnchantedPermanent>>> Object { get; set; }
}
