namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hurl into History, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hurl into History</exampledoc>
[Dependent]
public class HurlIntoHistoryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile cards from the top of your library until you exile a nonland card with that mana value or less"];
}
