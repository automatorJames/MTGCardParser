namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Extirpate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Extirpate</exampledoc>
[TokenizationOrder(-1)]
public class ExtirpateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target card in a graveyard other than a basic land card"];
}
