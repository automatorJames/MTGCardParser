namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ignis Scientia, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ignis Scientia</exampledoc>
[TokenizationOrder(-1)]
public class IgnisScientiaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["i've come up with a new recipe! — {1}{g}{u}, {t}: exile target card from a graveyard"];
}
