namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Grim Reminder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grim Reminder</exampledoc>
[TokenizationOrder(-1)]
public class GrimReminderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent who cast a spell this turn with the same name as that card loses 6 life"];
}
