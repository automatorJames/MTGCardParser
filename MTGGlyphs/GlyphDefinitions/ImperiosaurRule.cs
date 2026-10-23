namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Imperiosaur, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Imperiosaur</exampledoc>
[TokenizationOrder(-1)]
public class ImperiosaurRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend only mana produced by basic lands to cast this spell"];
}
