namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Sharing a name with an object named earlier: "with the same name as that creature", "with the same name as that spell", "with the same name as the exiled card", "with the same name as it", "with the same name as this spell".</summary>
/// <exampledoc>Echoing Ruin</exampledoc>
/// <examplecapture>with the same name as that artifact</examplecapture>
[Dependent]
public class WithSameName : Glyph
{
    public override Nib[] Nibs => ["with the same name as", Prop(Object)];

    public OneOf<OneOf<ThatCardPhrase, ThatCard, ThatStackObject>, OneOf<SacrificedPermanent, ThisSpell>, It> Object { get; set; }
}
