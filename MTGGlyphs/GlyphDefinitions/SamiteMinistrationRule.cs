namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Samite Ministration, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Samite Ministration</exampledoc>
[TokenizationOrder(-1)]
public class SamiteMinistrationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever damage from a black or red source is prevented this way this turn, you gain that much life"];
}
