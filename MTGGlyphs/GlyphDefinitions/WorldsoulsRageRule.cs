namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Worldsoul's Rage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Worldsoul's Rage</exampledoc>
[TokenizationOrder(-1)]
public class WorldsoulsRageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put up to x land cards from your hand and/or graveyard onto the battlefield tapped"];
}
