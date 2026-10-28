namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Sharing a name with an object named earlier: "with the same name as that creature", "with the same name as that spell", "with the same name as the exiled card", "with the same name as it", "with the same name as this spell", or with any one of a kind: "with the same name as another creature you control", "with the same name as a card in your graveyard", "with the same name as a card exiled with {this}".</summary>
/// <exampledoc>Echoing Ruin</exampledoc>
/// <examplecapture>with the same name as that artifact</examplecapture>
[Dependent]
public class WithSameName : Glyph
{
    public override Nib[] Nibs => ["with the same name as", Prop(Object)];

    public OneOf<ThatCardPhrase, ThatCard, ThatStackObject, SacrificedPermanent, ThisSpell, It, NamesakePermanent, NamesakeCard, CardsExiledWithThis> Object { get; set; }
}
