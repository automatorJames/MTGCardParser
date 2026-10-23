namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nether Shadow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nether Shadow</exampledoc>
/// <examplecapture>if this card is in your graveyard with three or more creature cards above it, you may put this card onto the battlefield</examplecapture>
[Dependent]
public class NetherShadowEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this card is in your graveyard with three or more creature cards above it, you may put this card onto the battlefield"];
}
