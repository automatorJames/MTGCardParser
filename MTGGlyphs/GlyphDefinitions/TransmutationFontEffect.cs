namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Transmutation Font, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Transmutation Font</exampledoc>
[Dependent]
public class TransmutationFontEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create your choice of a blood token, a clue token, or a food token"];
}
