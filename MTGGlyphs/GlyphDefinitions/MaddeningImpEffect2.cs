namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Maddening Imp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Maddening Imp</exampledoc>
[Dependent]
public class MaddeningImpEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy each of those creatures that didn't attack this turn"];
}
