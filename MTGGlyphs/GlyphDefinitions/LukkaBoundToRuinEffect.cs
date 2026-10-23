namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lukka, Bound to Ruin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lukka, Bound to Ruin</exampledoc>
/// <examplecapture>lukka deals x damage divided as you choose among any number of target creatures and/or planeswalkers, where x is the greatest power among creatures you control as you activate this ability</examplecapture>
[Dependent]
public class LukkaBoundToRuinEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lukka deals x damage divided as you choose among any number of target creatures and/or planeswalkers, where x is the greatest power among creatures you control as you activate this ability"];
}
