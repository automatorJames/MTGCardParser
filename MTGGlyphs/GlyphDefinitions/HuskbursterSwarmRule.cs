namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Huskburster Swarm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Huskburster Swarm</exampledoc>
[TokenizationOrder(-1)]
public class HuskbursterSwarmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this spell costs {1} less to cast for each creature card you own in exile and in your graveyard"];
}
