namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sanctum Lurker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sanctum Lurker</exampledoc>
[TokenizationOrder(-1)]
public class SanctumLurkerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["planeswalkers you control aren't put into their owners' graveyards for having 0 loyalty"];
}
