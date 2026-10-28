namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Assimilation Aegis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Assimilation Aegis</exampledoc>
[TokenizationOrder(-1)]
public class AssimilationAegisRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this equipment becomes attached to a creature, for as long as this equipment remains attached to it, that creature becomes a copy of a creature card exiled with this equipment"];
}
