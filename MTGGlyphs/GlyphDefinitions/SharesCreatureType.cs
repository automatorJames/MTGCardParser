namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that share a creature type with it", "that shares a creature type with equipped creature", "that shares a color with it", "that shares a card type with the sacrificed permanent", "that shares a card type with the chosen card": permanents narrowed to those sharing a creature type, card type or color with another object (kinship-style tribal effects, radiance, conspire).</summary>
/// <exampledoc>Konda's Banner</exampledoc>
/// <examplecapture>that share a creature type with equipped creature</examplecapture>
[Dependent]
public class SharesCreatureType : Glyph
{
    public override Nib[] Nibs => ["that", Pattern("shares?"), Prop(Aspect), "with", Prop(With)];

    public SharedAspect Aspect { get; set; }
    public OneOf<OneOf<It, ThisCreature, EnchantedPermanent>, SacrificedPermanent, ThatCardPhrase> With { get; set; }
}
