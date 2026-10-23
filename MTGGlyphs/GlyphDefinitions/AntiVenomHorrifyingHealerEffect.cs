namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Anti-Venom, Horrifying Healer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Anti-Venom, Horrifying Healer</exampledoc>
[Dependent]
public class AntiVenomHorrifyingHealerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if he was cast, return target creature card from your graveyard to the battlefield"];
}
