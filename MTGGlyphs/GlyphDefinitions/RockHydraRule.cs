namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rock Hydra, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rock Hydra</exampledoc>
/// <examplecapture>for each 1 damage that would be dealt to this creature, if it has a +1/+1 counter on it, remove a +1/+1 counter from it and prevent that 1 damage</examplecapture>
[TokenizationOrder(-1)]
public class RockHydraRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each 1 damage that would be dealt to this creature, if it has a +1/+1 counter on it, remove a +1/+1 counter from it and prevent that 1 damage"];
}
