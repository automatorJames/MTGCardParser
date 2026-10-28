namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Oviya, Automech Artisan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oviya, Automech Artisan</exampledoc>
[TokenizationOrder(-1)]
public class OviyaAutomechArtisanRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature that's attacking one of your opponents has trample"];
}
