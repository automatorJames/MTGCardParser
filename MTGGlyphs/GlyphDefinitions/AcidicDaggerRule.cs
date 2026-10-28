namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Acidic Dagger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Acidic Dagger</exampledoc>
[TokenizationOrder(-1)]
public class AcidicDaggerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when the targeted creature leaves the battlefield this turn, sacrifice this artifact"];
}
