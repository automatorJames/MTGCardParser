namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Intimidation Campaign, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Intimidation Campaign</exampledoc>
[Dependent]
public class IntimidationCampaignEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it returns only from the battlefield"];
}
