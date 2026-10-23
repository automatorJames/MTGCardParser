namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fortune, Loyal Steed, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fortune, Loyal Steed</exampledoc>
/// <examplecapture>at end of combat, exile it and up to one creature that saddled it this turn, then return those cards to the battlefield under their owner's control</examplecapture>
[Dependent]
public class FortuneLoyalSteedEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["at end of combat, exile it and up to one creature that saddled it this turn, then return those cards to the battlefield under their owner's control"];
}
