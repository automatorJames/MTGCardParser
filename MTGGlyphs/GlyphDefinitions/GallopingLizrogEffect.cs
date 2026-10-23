namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Galloping Lizrog, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Galloping Lizrog</exampledoc>
[Dependent]
public class GallopingLizrogEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may remove any number of +1/+1 counters from among creatures you control"];
}
