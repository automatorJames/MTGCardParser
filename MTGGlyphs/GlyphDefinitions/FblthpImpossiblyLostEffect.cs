namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fblthp, Impossibly Lost, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fblthp, Impossibly Lost</exampledoc>
[Dependent]
public class FblthpImpossiblyLostEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you draw from an empty library this way, you still win the game"];
}
