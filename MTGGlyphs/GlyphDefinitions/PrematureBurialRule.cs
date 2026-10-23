namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Premature Burial, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Premature Burial</exampledoc>
[TokenizationOrder(-1)]
public class PrematureBurialRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target nonblack creature that entered since your last turn ended"];
}
