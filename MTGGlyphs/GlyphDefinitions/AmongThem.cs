namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"among them", "among those cards", "among the milled cards", "among the revealed cards", "among cards exiled with this creature": out of the cards just named, or set aside earlier.</summary>
/// <exampledoc>Animal Magnetism</exampledoc>
/// <examplecapture>among them</examplecapture>
[Dependent]
public class AmongThem : Glyph
{
    public override Nib[] Nibs => ["among", Prop(Cards)];

    public OneOf<Them, Those, OneOf<SacrificedPermanent, CardsExiledWithThis>> Cards { get; set; }
}
