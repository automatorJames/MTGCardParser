namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Blade of the Swarm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blade of the Swarm</exampledoc>
[TokenizationOrder(-1)]
public class BladeOfTheSwarmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• put target exiled card with warp on the bottom of its owner's library"];
}
