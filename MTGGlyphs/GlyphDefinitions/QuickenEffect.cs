namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Quicken, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quicken</exampledoc>
[Dependent]
public class QuickenEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it can be cast any time you could cast an instant"];
}
