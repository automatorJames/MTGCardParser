namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cryptic Gateway, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cryptic Gateway</exampledoc>
/// <examplecapture>you may put a creature card from your hand that shares a creature type with each creature tapped this way onto the battlefield</examplecapture>
[Dependent]
public class CrypticGatewayEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put a creature card from your hand that shares a creature type with each creature tapped this way onto the battlefield"];
}
