namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Yidaro, Wandering Monster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yidaro, Wandering Monster</exampledoc>
[Dependent]
public class YidaroWanderingMonsterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle it into your library from your graveyard"];
}
