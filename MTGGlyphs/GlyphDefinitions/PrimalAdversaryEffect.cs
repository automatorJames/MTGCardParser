namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Primal Adversary, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Primal Adversary</exampledoc>
/// <examplecapture>put that many +1/+1 counters on this creature, then up to that many target lands you control become 3/3 wolf creatures with haste that are still lands</examplecapture>
[Dependent]
public class PrimalAdversaryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put that many +1/+1 counters on this creature, then up to that many target lands you control become 3/3 wolf creatures with haste that are still lands"];
}
