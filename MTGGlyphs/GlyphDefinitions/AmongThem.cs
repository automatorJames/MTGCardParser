namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"among them", "among those cards": out of the cards just named.</summary>
/// <exampledoc>Animal Magnetism</exampledoc>
/// <examplecapture>among them</examplecapture>
[Dependent]
public class AmongThem : Glyph
{
    public override Nib[] Nibs => ["among", Prop(Cards)];

    public OneOf<Them, Those> Cards { get; set; }
}
