namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Meeting of the Five, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Meeting of the Five</exampledoc>
[TokenizationOrder(-1)]
public class MeetingOfTheFiveRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only to cast spells with exactly three colors"];
}
