namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dyadrine, Synthesis Amalgam, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dyadrine, Synthesis Amalgam</exampledoc>
[Dependent]
public class DyadrineSynthesisAmalgamEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may remove a +1/+1 counter from each of two creatures you control"];
}
