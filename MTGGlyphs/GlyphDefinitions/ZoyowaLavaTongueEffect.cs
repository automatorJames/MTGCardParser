namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Zoyowa Lava-Tongue, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zoyowa Lava-Tongue</exampledoc>
[Dependent]
public class ZoyowaLavaTongueEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you descended this turn, each opponent may discard a card or sacrifice a permanent of their choice"];
}
