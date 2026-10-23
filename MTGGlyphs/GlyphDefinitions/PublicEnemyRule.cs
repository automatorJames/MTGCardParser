namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Public Enemy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Public Enemy</exampledoc>
[TokenizationOrder(-1)]
public class PublicEnemyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all creatures attack enchanted creature's controller each combat if able"];
}
