namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of General Tazri, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>General Tazri</exampledoc>
[Dependent]
public class GeneralTazriEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ally creatures you control get +x/+x until end of turn, where x is the number of colors among those creatures"];
}
