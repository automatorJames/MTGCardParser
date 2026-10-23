namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Maddening Imp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Maddening Imp</exampledoc>
[Dependent]
public class MaddeningImpCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["non-wall creatures the active player controls attack this turn if able"];
}
