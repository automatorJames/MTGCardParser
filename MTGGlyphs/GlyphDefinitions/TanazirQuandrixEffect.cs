namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tanazir Quandrix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tanazir Quandrix</exampledoc>
/// <examplecapture>you may have the base power and toughness of other creatures you control become equal to {this}'s power and toughness until end of turn</examplecapture>
[Dependent]
public class TanazirQuandrixEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have the base power and toughness of other creatures you control become equal to", Nib.This, "'s power and toughness until end of turn"];
}
