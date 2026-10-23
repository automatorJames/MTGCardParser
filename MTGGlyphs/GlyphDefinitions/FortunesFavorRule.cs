namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fortune's Favor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fortune's Favor</exampledoc>
[TokenizationOrder(-1)]
public class FortunesFavorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent looks at the top four cards of your library and separates them into a face-down pile and a face-up pile"];
}
