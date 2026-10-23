namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Master Symmetrist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master Symmetrist</exampledoc>
[Dependent]
public class MasterSymmetristEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gains trample until end of turn"];
}
