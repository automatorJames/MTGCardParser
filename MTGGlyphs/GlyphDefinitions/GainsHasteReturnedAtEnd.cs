namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Haste-then-return tail of a temporary reanimation: "if you do, it gains haste, and it's returned from the battlefield to its owner's hand at the beginning of the next end step".</summary>
/// <exampledoc>Mardu Scout</exampledoc>
/// <examplecapture>if you do, it gains haste, and it's returned from the battlefield to its owner's hand at the beginning of the next end step</examplecapture>
public class GainsHasteReturnedAtEnd : Glyph
{
    public override Nib[] Nibs => ["if you do, it gains haste, and it's returned from the battlefield to its owner's hand at the beginning of the next end step"];
}
