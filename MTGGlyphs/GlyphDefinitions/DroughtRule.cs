namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Drought, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Drought</exampledoc>
[TokenizationOrder(-1)]
public class DroughtRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activated abilities cost an additional \"sacrifice a swamp\" to activate for each black mana symbol in their activation costs"];
}
