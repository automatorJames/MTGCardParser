namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Woodcaller Automaton, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Woodcaller Automaton</exampledoc>
[TokenizationOrder(-1)]
public class WoodcallerAutomatonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it becomes a treefolk creature with haste and base power and toughness equal to this creature's power and toughness"];
}
