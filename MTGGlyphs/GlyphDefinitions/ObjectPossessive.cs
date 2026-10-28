namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object's possessive: "that creature's", "{this}'s", "this creature's", "the sacrificed artifact's", "the discarded card's", "that card's", "that spell's", "enchanted creature's", "that source's", or a legend's short name ("syr faren's"). The spell reference goes before ThisCreature, whose bare "this" would stop short of "this spell's"; "that card"/"the exiled card" go before handled objects; the short name goes last.</summary>
/// <exampledoc>Diamond Valley</exampledoc>
/// <examplecapture>the sacrificed creature's</examplecapture>
[Dependent]
public class ObjectPossessive : Glyph
{
    public override Nib[] Nibs => [Prop(Object), "'s"];

    public OneOf<This, ThatCard, ThatCardPhrase, SacrificedPermanent, SpellReference?, ThisCreature, EnchantedPermanent, ThatSource, PossessedShortName> Object { get; set; }
}
