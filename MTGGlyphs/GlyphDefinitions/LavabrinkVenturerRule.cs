namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lavabrink Venturer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lavabrink Venturer</exampledoc>
[TokenizationOrder(-1)]
public class LavabrinkVenturerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has protection from each mana value of the chosen quality"];
}
