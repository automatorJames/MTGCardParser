namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Beholding: showing a permanent of a kind you control or a card of that kind in your hand - "behold a dragon", "behold an elf", "behold three elementals". A predicate ("you may behold a dragon") and a cost ("behold a jace or pay {1}").</summary>
/// <exampledoc>Sarkhan, Dragon Ascendant</exampledoc>
/// <examplecapture>behold a dragon</examplecapture>
public class Behold : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["behold", Prop(What)];

    public OneOf<ArticleKind, CountedKinds> What { get; set; }
}
