namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Oviya, Automech Artisan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oviya, Automech Artisan</exampledoc>
[TokenizationOrder(-1)]
public class OviyaAutomechArtisanRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you put an artifact onto the battlefield this way, put two +1/+1 counters on it"];
}
