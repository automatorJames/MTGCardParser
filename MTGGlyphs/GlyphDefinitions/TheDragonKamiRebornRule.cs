namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Dragon-Kami Reborn // Dragon-Kami's Egg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Dragon-Kami Reborn // Dragon-Kami's Egg</exampledoc>
[TokenizationOrder(-1)]
public class TheDragonKamiRebornRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile one of them face down with a hatching counter on it, then put the rest on the bottom of your library in any order"];
}
