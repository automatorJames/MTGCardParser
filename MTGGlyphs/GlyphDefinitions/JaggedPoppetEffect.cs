namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jagged Poppet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jagged Poppet</exampledoc>
/// <examplecapture>if you have no cards in hand, that player discards cards equal to the damage</examplecapture>
[Dependent]
public class JaggedPoppetEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you have no cards in hand, that player discards cards equal to the damage"];
}
