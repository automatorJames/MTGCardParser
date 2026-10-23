namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shoot the Sheriff, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shoot the Sheriff</exampledoc>
[TokenizationOrder(-1)]
public class ShootTheSheriffRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["everyone else is fair game"];
}
