namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Explosive Revelation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Explosive Revelation</exampledoc>
[TokenizationOrder(-1)]
public class ExplosiveRevelationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put the nonland card into your hand and the rest on the bottom of your library in any order"];
}
