namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Undergrowth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Undergrowth</exampledoc>
[TokenizationOrder(-1)]
public class UndergrowthRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell's additional cost was paid, this effect doesn't affect combat damage that would be dealt by red creatures"];
}
