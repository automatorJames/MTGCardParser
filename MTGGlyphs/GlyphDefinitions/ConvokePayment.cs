namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Convoke's second reminder sentence.</summary>
/// <exampledoc>Autochthon Wurm</exampledoc>
/// <examplecapture>each creature you tap while casting this spell pays for {1} or one mana of that creature's color</examplecapture>
public class ConvokePayment : Glyph
{
    public override Nib[] Nibs => ["each creature you tap while casting this spell pays for {1} or one mana of that creature's color"];
}
