namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Intuition, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Intuition</exampledoc>
[TokenizationOrder(-1)]
public class IntuitionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent chooses one"];
}
