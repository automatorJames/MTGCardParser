namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Goblin Lyre, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Lyre</exampledoc>
[TokenizationOrder(-1)]
public class GoblinLyreRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lose the flip, this artifact deals damage to you equal to the number of creatures that opponent or that planeswalker's controller controls"];
}
