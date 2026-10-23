namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cairn Wanderer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cairn Wanderer</exampledoc>
[TokenizationOrder(-1)]
public class CairnWandererRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as a creature card with flying is in a graveyard, this creature has flying"];
}
