namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Daring Thief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Daring Thief</exampledoc>
/// <examplecapture>you may exchange control of target nonland permanent you control and target permanent an opponent controls that shares a card type with it</examplecapture>
[Dependent]
public class DaringThiefEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may exchange control of target nonland permanent you control and target permanent an opponent controls that shares a card type with it"];
}
