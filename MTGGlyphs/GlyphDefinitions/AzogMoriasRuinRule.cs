namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Azog, Moria's Ruin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Azog, Moria's Ruin</exampledoc>
[TokenizationOrder(-1)]
public class AzogMoriasRuinRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they don't control an army, they create a 0/0 black goblin army creature token first"];
}
