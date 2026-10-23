namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Teferi, Master of Time, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi, Master of Time</exampledoc>
[Dependent]
public class TeferiMasterOfTimeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["take two extra turns after this one"];
}
