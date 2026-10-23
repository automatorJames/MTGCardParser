namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Final Punishment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Final Punishment</exampledoc>
[TokenizationOrder(-1)]
public class FinalPunishmentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player loses life equal to the damage already dealt to that player this turn"];
}
