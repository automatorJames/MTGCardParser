namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Zoyowa Lava-Tongue, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zoyowa Lava-Tongue</exampledoc>
[TokenizationOrder(-1)]
public class ZoyowaLavaTongueRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["zoyowa deals 3 damage to each opponent who didn't"];
}
