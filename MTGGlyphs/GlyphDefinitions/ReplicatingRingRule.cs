namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Replicating Ring, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Replicating Ring</exampledoc>
/// <examplecapture>then if it has eight or more night counters on it, remove all of them and create eight colorless snow artifact tokens named replicated ring with "{t}: add one mana of any color</examplecapture>
[TokenizationOrder(-1)]
public class ReplicatingRingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if it has eight or more night counters on it, remove all of them and create eight colorless snow artifact tokens named replicated ring with \"{t}: add one mana of any color"];
}
