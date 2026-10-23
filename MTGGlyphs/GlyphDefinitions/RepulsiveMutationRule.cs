namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Repulsive Mutation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Repulsive Mutation</exampledoc>
[TokenizationOrder(-1)]
public class RepulsiveMutationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then counter up to one target spell unless its controller pays mana equal to the greatest power among creatures you control"];
}
