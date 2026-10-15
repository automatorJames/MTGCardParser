namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Morph's first reminder sentence: "you may cast this card face down as a 2/2 creature for {3}".</summary>
/// <exampledoc>Aquamorph Entity</exampledoc>
/// <examplecapture>you may cast this card face down as a 2/2 creature for {3}</examplecapture>
[Dependent]
public class CastFaceDown : Glyph
{
    public override Nib[] Nibs => ["you may cast this card face down as a 2/2 creature for", Prop(Cost)];

    public ManaCost Cost { get; set; }
}
