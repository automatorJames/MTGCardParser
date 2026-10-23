namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mangara's Blessing, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mangara's Blessing</exampledoc>
/// <examplecapture>you gain 2 life, and you return this card from your graveyard to your hand at the beginning of the next end step</examplecapture>
[Dependent]
public class MangarasBlessingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain 2 life, and you return this card from your graveyard to your hand at the beginning of the next end step"];
}
