namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ral Zarek, Guest Lecturer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ral Zarek, Guest Lecturer</exampledoc>
[TokenizationOrder(-1)]
public class RalZarekGuestLecturerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent skips their next x turns, where x is the number of coins that came up heads"];
}
